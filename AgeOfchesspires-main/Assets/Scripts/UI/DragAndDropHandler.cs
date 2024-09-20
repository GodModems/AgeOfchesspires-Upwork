using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static RoomPlayer;

public class DragAndDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Canvas canvas;
    private MultiplayerGamePlay multiplayerGame;

    private void Start()
    {
        // Get the parent Canvas for handling the drag on the UI layer
        canvas = GetComponentInParent<Canvas>();
        multiplayerGame = FindObjectOfType<MultiplayerGamePlay>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (multiplayerGame._gameState == MultiplayerGameState.ONGOING)
        {
            if (multiplayerGame._state == MultiplayerPlayState.USER_MOVE || multiplayerGame._state == MultiplayerPlayState.USER_WAIT_PROMOTE)
            {
            

                // Store the original parent to return the button if needed
                originalParent = transform.parent;

                // Set the button to be under the Canvas root to drag freely
                transform.SetParent(canvas.transform, true);

                //GameObject objectUnderPointer = GetObjectUnderPointer(eventData);
                //GameObject objectBelowTopmost = GetObjectBelowTopmost(eventData);
                GameObject.FindObjectOfType<MultiplayerGamePlay>()
                    .handleClickToUserPlayers(transform.GetComponent<Button>());

                //GameObject.FindObjectOfType<MultiplayerGamePlay>().getAvailableMoveAndColourThem(transform.GetComponent<Button>(), 1);

                // Optional: Disable raycast blocking so it doesn't interfere during drag
                //GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (multiplayerGame._gameState == MultiplayerGameState.ONGOING)
        {
            if (multiplayerGame._state == MultiplayerPlayState.USER_MOVE || multiplayerGame._state == MultiplayerPlayState.USER_WAIT_PROMOTE)
            {
                // Move the button with the mouse/finger drag
                transform.position = eventData.position;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (multiplayerGame._gameState == MultiplayerGameState.ONGOING)
        {
            if (multiplayerGame._state == MultiplayerPlayState.USER_MOVE || multiplayerGame._state == MultiplayerPlayState.USER_WAIT_PROMOTE)
            {
                //GameObject objectUnderPointer = GetObjectUnderPointer(eventData);
                GameObject objectBelowTopmost = GetObjectBelowTopmost(eventData);

                if (objectBelowTopmost != null)
                {
                    Debug.Log("Object Below Top Most:" + objectBelowTopmost.name);

                    GridInfo gridInfo = objectBelowTopmost.GetComponent<GridInfo>();

                    if (gridInfo != null)
                    {
                        CharacterDef characterDef =
                            gridInfo.characterDef;
                        bool isOpp = false;
                        if (characterDef != CharacterDef.None)
                        {
                            isOpp = true;
                        }

                        // Handle dropping logic
                        DropButton(objectBelowTopmost, isOpp);
                    }
                    else
                    {
                        ResetPlace();
                        ResetHighligts();
                    }
                }
                else
                {
                    ResetPlace();
                    ResetHighligts();
                }
            }
            
        }
    }

    private void DropButton(GameObject gobj, bool isOpp)
    {

        Debug.Log("Dropping Now");
        if (gobj != null)
        {
            //GameObject gobj = eventData.pointerEnter.transform.gameObject;

            Debug.Log(gobj.GetComponent<GridInfo>().x + ":Game Obj Under Pointer:"+ gobj.GetComponent<GridInfo>().y);

            GridInfo gridInfo = gobj.GetComponent<GridInfo>();

            Debug.Log(gridInfo.x + ":Move TO:" + gridInfo.y);

            bool isValid = IsCorrectPlace(gridInfo);
            if (isValid)
            {
                Debug.Log("Is Valid True");
                if(!isOpp)
                {
                    GameObject.FindObjectOfType<MultiplayerGamePlay>().
                    HandleClickToTheBoard(gobj.GetComponent<Button>());
                }
                else
                {
                    GameObject.FindObjectOfType<MultiplayerGamePlay>().
                    handleClickToOpponent(gobj.GetComponent<Button>());
                }
                
                ResetHighligts();

            }
            else
            {
                Debug.Log("Is Valid False");
                ResetPlace();
                ResetHighligts();

            }
        }
        else
        {
            ResetPlace();
            ResetHighligts();
        }

    }

    public void ResetPlace()
    {
        // Return the button to the original parent if it was not dropped on a valid target
        transform.SetParent(originalParent, true);
        transform.localPosition = Vector3.zero; // Center the button in the new grid
        
    }

    public void ResetHighligts()
    {
        GameObject.FindObjectOfType<MultiplayerGamePlay>().charHandler.RemoveHighLightColour();
        GameObject.FindObjectOfType<MultiplayerGamePlay>().GetMoveSelectionList().Clear();
    }

    private bool IsCorrectPlace(GridInfo gridInfo)
    {
        bool isValid = false;
        List<Char_Point> moveSelectionList = GameObject.FindObjectOfType<MultiplayerGamePlay>().GetMoveSelectionList();
        foreach (Char_Point move in moveSelectionList)
        {
            Debug.Log(move.x + ":Move TO From List:" + move.y);
            Debug.Log(gridInfo.x + ":Move TO 3:" + gridInfo.y);


            if (move.x == gridInfo.x && move.y == gridInfo.y)
            {
                isValid = true; 
                break;
            }
        }

        return isValid;
    }

    /*private GameObject GetObjectUnderPointer(PointerEventData eventData)
    {
        // Create a list to hold raycast results
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // Perform a raycast using the event system's raycaster
        EventSystem.current.RaycastAll(eventData, raycastResults);

        // Return the first valid GameObject hit by the raycast, if any
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject != null)
            {
                return result.gameObject;
            }
        }

        // If no object was found under the pointer, return null
        return null;
    }*/

    private GameObject GetObjectBelowTopmost(PointerEventData eventData)
    {
        // Create a list to hold raycast results
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // Perform a raycast using the event system's raycaster
        EventSystem.current.RaycastAll(eventData, raycastResults);

        // Ensure there is more than one result
        if (raycastResults.Count > 1)
        {
            // Return the second GameObject in the raycast results list (below the topmost object)
            if(raycastResults[1].gameObject.name.Equals("bw"))
            {
                return raycastResults[2].gameObject;
            }
            return raycastResults[1].gameObject;
        }

        // If there's no object below the topmost object, return null
        return null;
    }
}
