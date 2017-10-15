using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// http://answers.unity3d.com/questions/658307/rotate-camera-around-object-1.html
public class OrbitCamera : MonoBehaviour {
    public Transform target;
 public float distance = 5.0f;
 public float xSpeed = 60.0f;
 public float ySpeed = 60.0f;
 public float yMinLimit = 10f;
 public float yMaxLimit = 60f;
 private static readonly float[] ZoomBounds = new float[]{10f, 85f};
 float x = 0.0f;
 float y = 0.0f;

 private Camera cam;

 private bool wasZoomingLastFrame; // Touch mode only
 private Vector2[] lastZoomPositions;

 void Awake() {
     cam = GetComponent<Camera>();
 }

 void Start()
 {
     // https://docs.unity3d.com/ScriptReference/Transform.LookAt.html
     transform.LookAt(target);
     Vector3 angles = transform.eulerAngles;
     x = angles.y;
     y = angles.x;
 }

 void Update()
 {
     if (target && Input.touchCount == 1) 
     {
        wasZoomingLastFrame = false;
            Debug.Log("One Touch");
        if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Orbit(Input.GetTouch(0));
        }
        if (Input.GetTouch(0).phase == TouchPhase.Began) 
        {
            // http://answers.unity3d.com/questions/641253/how-to-select-an-object-with-touch-and-change-its.html
            Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            Debug.Log("Hello!");
            GameObject.Find("TheBox").SendMessage("Clicked",0,SendMessageOptions.DontRequireReceiver);
            // http://answers.unity3d.com/questions/610440/on-touch-event-on-game-object-on-android-2d.html
            if (hit) {
                Debug.Log(hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("Clicked",0,SendMessageOptions.DontRequireReceiver);
            }
        }
     }
     else if (Input.touchCount == 2)
     {
         // https://kylewbanks.com/blog/unity3d-panning-and-pinch-to-zoom-camera-with-touch-and-mouse-input
         Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
         if (!wasZoomingLastFrame) {
             wasZoomingLastFrame = true;
         } else {
             float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
             float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
             float offset = newDistance - oldDistance;
             ZoomCamera(offset, 0.1f);
         }
         lastZoomPositions = newPositions;
     }
      else {
          wasZoomingLastFrame = false;
      }
 }

 void Orbit(Touch touch)
 {
     x += touch.deltaPosition.x * xSpeed * 0.02f /* * distance*/;
     y -= touch.deltaPosition.y * ySpeed * 0.02f /* * distance*/;
     y = ClampAngle(y, yMinLimit, yMaxLimit);
     Quaternion rotation = Quaternion.Euler(y, x, 0);
     Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
     Vector3 position = rotation * negDistance + target.position;
     transform.rotation = rotation;
     transform.position = position;
 }

 public static float ClampAngle(float angle, float min, float max)
 {
     if (angle < -360F)
         angle += 360F;
     if (angle > 360F)
         angle -= 360F;
     return Mathf.Clamp(angle, min, max);
 }

    void ZoomCamera(float offset, float speed) {
        if (offset == 0) {
            return;
        }
    
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }
}
