using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class BallonInflator : XRGrabInteractable
{
    // Start is called before the first frame update

    [Header("Ballon Data")]
    public Transform attachPoint;
    public Balloon ballonPrefab;
    private Balloon m_BalloonInstance; // new variable
    private XRBaseController m_Controller;
    
    //attach ballon find which hand the ballon in handled
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        m_BalloonInstance = Instantiate(ballonPrefab,attachPoint);
        var controllerInteractor = args.interactorObject as XRBaseControllerInteractor;
        m_Controller = controllerInteractor.xrController;

        m_Controller.SendHapticImpulse(1, 0.5f); // add this line
        Debug.Log(m_Controller);


    }

    //detach ballon
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        Destroy(m_BalloonInstance.gameObject);
    }

    //Scale the ballom
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if(isSelected && m_Controller != null)
        {
            m_BalloonInstance.transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 4.0f,m_Controller.activateInteractionState.value);

        }
    }
}
