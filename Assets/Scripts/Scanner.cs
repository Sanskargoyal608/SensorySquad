using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class Scanner : XRGrabInteractable
{
    [Header("Scanner Data")]
    public Animator animator;
    public LineRenderer laserRenderer;
    public TextMeshProUGUI targetName;
    public TextMeshProUGUI targetPosition;
    private GameObject lastHitObject = null;
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        animator.SetBool("Opened", true);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        animator.SetBool("Opened", false);
    }

    protected override void Awake()
    {
        base.Awake();
        ScannerActivated(false);
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        ScannerActivated(true);
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        ScannerActivated(false);
    }

    private void ScannerActivated(bool isActivated)
    {
        laserRenderer.gameObject.SetActive(isActivated);
        targetName.gameObject.SetActive(isActivated);
        targetPosition.gameObject.SetActive(isActivated);
    }

    private void ScanForObject()
    {
        RaycastHit hit;
        Vector3 worldHit = laserRenderer.transform.position + laserRenderer.transform.forward * 1000.0f;
        if (Physics.Raycast(laserRenderer.transform.position, laserRenderer.transform.forward, out hit))
        {
            worldHit = hit.point;
            GameObject hitObject = hit.collider.gameObject;
            targetName.SetText(hit.collider.name); //add this
            targetPosition.SetText(hit.collider.transform.position.ToString());
            Renderer renderer = hitObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                // Check if a coroutine is already running for this object
                if (activeCoroutines.ContainsKey(hitObject))
                {
                    StopCoroutine(activeCoroutines[hitObject]);
                }

                // Start a new coroutine to handle the color change
                activeCoroutines[hitObject] = StartCoroutine(ChangeColorTemporarily(renderer, hitObject));
            }
        }
        laserRenderer.SetPosition(1, laserRenderer.transform.InverseTransformPoint(worldHit));
    }

    private IEnumerator ChangeColorTemporarily(Renderer renderer, GameObject hitObject)
    {
        // Store the original material
        Material originalMaterial = renderer.material;

        // Change the color to red
        renderer.material.color = Color.red;

        // Wait for 5 seconds
        yield return new WaitForSeconds(5.0f);

        // Restore the original material
        renderer.material = originalMaterial;

        // Remove the coroutine reference from the dictionary
        activeCoroutines.Remove(hitObject);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (laserRenderer.gameObject.activeSelf) // new if-statement
            ScanForObject(); // new line
    }
}
