using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickController : MonoBehaviour
{
    SelectionController selectionController;
    private void Start()
    {
        selectionController = GetComponent<SelectionController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            var clickTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var character in selectionController.PlayableCharacters)
            {
                if (!character.GetComponent<SelectableObject>().IsSelected)
                {
                    continue;
                }
                var moveTarget = new Vector3(clickTarget.x, clickTarget.y, character.transform.position.z);
                character.GetComponent<MovementController>().MoveToPosition(moveTarget);
            }
        }
    }
}
