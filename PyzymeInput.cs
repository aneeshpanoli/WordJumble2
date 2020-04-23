using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PyzymeInput
{
    public static Tuple<string, Transform> hit_object_transform(){
        string input_str = "";
        Transform hit_transform = null;
        Vector2 input_pos = Vector2.zero;
        if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch (0); // only considers the first touch
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // HandleTouchBegan (touch.position);
                        input_str = "Began";
                        hit_transform = get_hit_transform(touch.position);
                        break;
                    case TouchPhase.Moved:
                        // HandleTouchMoved (touch.position);
                        input_str = "Moved";
                        hit_transform = get_hit_transform(touch.position);
                        break;
                    case TouchPhase.Ended:
                        hit_transform = get_hit_transform(touch.position);
                        input_str = "Ended";
                        break;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown (0))
                {
                    input_str = "Began";
                    hit_transform = get_hit_transform(Input.mousePosition);
                }

                else if (Input.GetMouseButtonUp (0))
                {
                input_str = "Ended";
                hit_transform = get_hit_transform(Input.mousePosition);
                }
            }
            return Tuple.Create(input_str, hit_transform);
        }

    private static Transform get_hit_transform(Vector2 pos){
        RaycastHit2D hit = Physics2D.Raycast(
                        Camera.main.ScreenToWorldPoint(pos),
                        Vector2.zero
                    );
        if (hit){
            return hit.transform;
        }
        return null;
    }
}
