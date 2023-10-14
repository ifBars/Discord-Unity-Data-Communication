public class DiscordStatsManager : MonoBehaviour
{
    public bool doSend = true;
    public string webhookUrl;
    private float saveCooldown = 10f; // Cooldown time in seconds
    private float lastSaveTime = 0f;
    private bool isSending = false;

    private void Start()
    {
        lastSaveTime = Time.time - saveCooldown;
    }

    private IEnumerator PostToDiscordCoroutine(string message)
    {
        if (Time.time - lastSaveTime >= saveCooldown && !isSending)
        {
            using (WebClient client = new WebClient())
            {
                isSending = true;
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string payload = $"content={UnityWebRequest.EscapeURL(message)}";

                // Use UploadStringAsync to send the payload asynchronously
                var uploadTask = client.UploadStringTaskAsync(webhookUrl, payload);

                // Wait for the upload to complete
                yield return new WaitUntil(() => uploadTask.IsCompleted);

                // Handle any errors here if needed
                if (uploadTask.IsFaulted)
                {
                    Debug.LogError("Error sending message to Discord: " + uploadTask.Exception);
                }
                isSending = false;
                lastSaveTime = Time.time;
            }
        }
    }
}