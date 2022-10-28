using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    private const float DEADZONE = 100.0f; // deadzone is the pixels we have to slide by, before it's recognized as one. anything less is not considered a swipe. 
    
    private static MobileInput instance;
    public static MobileInput Singleton { get { return instance; } }

    private Vector2 startTouch;
    public bool Tap { get; private set; }
    public bool SwipeLeft { get; private set; }
    public bool SwipeRight { get; private set; }
    public bool SwipeDown { get; private set; }
    public bool SwipeUp { get; private set; }
    public Vector2 SwipeDelta { get; private set; }

    private void Start()
    {
        instance = this; 
    }

    private void Update()
    {
        // at the beginning of very frame, reset all booleans. 
        Tap = SwipeLeft = SwipeRight = SwipeDown = SwipeUp = false;


        #region Standalone Input

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Tap = true;
        //    startTouch = Input.mousePosition;
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    startTouch = SwipeDelta = Vector2.zero; // vector2 remember because we can only slide on a 2d screen left and right. 
        //}
        #endregion

        #region Mobile Input

        if (Input.touches.Length != 0) // if we had atleast one touch
        {
            // touches[0] is the first touch on the screen. 
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                Tap = true;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) // canceled can occur when during touching, you received a phone call
                                                                                                                  // so the screen changes. 
            {
                startTouch = SwipeDelta = Vector2.zero; // vector2 remember because we can only slide on a 2d screen left/right and up/down (no forward/backward). 
            }
        }
        #endregion

        // calculate distance (delta) 
        SwipeDelta = Vector2.zero; // initialize. 

        // this condition checks to make sure we actually started touching somewhere. 
        if (startTouch != Vector2.zero)
        {
            // checking for Mobile. 
            if (Input.touches.Length != 0)
            {
                // delta is the difference of where we want to be, minus where we are currently at, basically. 
                SwipeDelta = Input.touches[0].position - startTouch;
            }
            // checking for PC
            else if (Input.GetMouseButton(0)) // we can't use down because we are doing a drag, we need to register all instances of the mousebutton at this time. 
            {
                SwipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        if (SwipeDelta.magnitude > DEADZONE)
        {
            //if we pass the deadzone we have a valid swipe
            float x = SwipeDelta.x;
            float y = SwipeDelta.y;

            // calculating swipe direction via our delta coordinates. 
            // basically if we are swiping in a direction, if the value of that direction is bigger than all others
            // then we are def. mov towards that direction. 
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                // means we are swiping LEFT/RIGHT
                if (x < 0)
                    SwipeLeft = true;
                else
                    SwipeRight = true;

                //remember all swipes will reset the next update frame. 
            }
            else
            {
                // means we are swiping UP/DOWN
                if (y < 0)
                    SwipeDown = true;
                else
                    SwipeUp = true;
            }
            // if we have had swipes, we don't want to have 2 of them in the same frame. so
            startTouch = SwipeDelta = Vector2.zero;
        }
    }
}
