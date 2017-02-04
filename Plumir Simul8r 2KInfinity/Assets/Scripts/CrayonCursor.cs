/******************************************
 * 
 * Created By: Andrew Decker
 * 
 * We want to have a cursor that is a crayon,
 * but we don't want to just replace the cursor
 * with a 2D image, so we'll track the mouse 
 * and place a crayon at its location. Our crayon
 * has a centralized pivot point, so we'll make an
 * empty gameobject for the tip and attach this
 * to it, then make the child the crayon model
 * 
 * ***************************************/

using UnityEngine;

public class CrayonCursor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        // Turn the cursor off
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Turn the mouse position into a real world position and move the crayon there
        Vector3 mousePos = Input.mousePosition;
        Vector3 desiredPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.9f));
        transform.position = desiredPos;
    }
}
