using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public XRController controller;
    public Transform mainActor;
    public Transform mainCamera;
    public Transform target;
    public GameObject prefab;
    public Slider slider;
    public Text message;
    private InputDevice m_InputDevice;
    private float timeElapsed = 0;
    private float userForce = 0;
    private bool lastTriggerred = false;
    // Start is called before the first frame update
    void Start()
    {
        m_InputDevice = InputDevices.GetDeviceAtXRNode(controller.controllerNode);
        slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("test left pos:" + controller.controllerNode.transform.position + ", dir:" +controller.transform.forward);
        if (m_InputDevice == null)
        {
            Debug.LogError("xxxxxxxx no device found on ");
            return;
        }
        // float m_TriggerValue;
        // m_InputDevice.TryGetFeatureValue(CommonUsages.trigger, out m_TriggerValue);
        // if(m_TriggerValue > 0){
        //     Debug.LogError("xxxxxxxx trigger " + m_TriggerValue);
        //     box.SetActive(false);
        // }
        bool triggered;
        m_InputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggered);
        Vector3 up = controller.transform.up;
        if(triggered == true && timeElapsed > 0.5){
            userForce += 10;
            lastTriggerred = true;
            slider.value = userForce / 2000.0f;
        }

        if(triggered == false && lastTriggerred == true){
            Debug.LogError("mytag: force " + userForce);
            GameObject box = Instantiate(prefab);
            box.transform.position = controller.transform.position;
            box.transform.Translate(controller.transform.forward * 2, Space.World);
            Rigidbody rigidBody = box.GetComponent<Rigidbody>();
            if(userForce > 2000){
                userForce = 2000;
            }
            rigidBody.AddForce(controller.transform.forward * userForce);
            timeElapsed = 0;
            userForce = 0;
            lastTriggerred = false;
            slider.value = 0;
        }

        else{
            Vector2 dir;
            m_InputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out dir);
            if(dir != Vector2.zero){
                bool forwardBack = Mathf.Abs(dir.y) > Mathf.Abs(dir.x);
                Vector3 orientation = mainCamera.forward;
                if(forwardBack)
                    orientation = dir.y > 0 ? mainCamera.forward : -mainCamera.forward;
                else
                    orientation = dir.x > 0 ? mainCamera.right : -mainCamera.right;
                orientation.y = 0;
                Debug.LogError("mytag: orientation " + orientation);
                mainActor.Translate(orientation * 0.1f);
            }

            Vector3 targetForward = target.position - mainActor.position;
            float dot = Vector3.Dot(targetForward, mainCamera.forward);
            Vector3 cross = Vector3.Cross(mainCamera.forward, targetForward);
            string s = "" + (dot > 0 ? "front" : "back") + " & " + (cross.y > 0 ? "right" : "left");
            message.text = s;
        }

        timeElapsed += Time.deltaTime;
    }
}
