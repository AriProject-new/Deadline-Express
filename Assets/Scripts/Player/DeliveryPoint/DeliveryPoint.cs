using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    [SerializeField] private GameObject interactPrompt; // Assign your "[E] Ring Bell" UI text here

    // We'll need a reference to the TypingManager to start the session.
    [SerializeField] private TypingManager typingManager;

    // We can also define the delivery sentence here per level.
    [SerializeField, TextArea(5, 10)]
    private string sentenceToType = "Paket berisi 1000 biji kelereng untuk Pak Budi di Gang Mawar.";

    private bool playerIsInRange = false;

    private void Awake()
    {
        // Ensure the prompt is hidden at the start.
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // This new log will tell us the name AND the tag of the colliding object.
        Debug.Log("Trigger entered by GameObject named '" + other.gameObject.name + "' with tag: '" + other.tag + "'");

        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // When the player leaves, hide the prompt.
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
    public void StartInteraction(Player player)
    {
        if (playerIsInRange)
        {
            typingManager.StartTypingSession(sentenceToType, player);
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }

}