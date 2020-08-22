using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    public bool isMoved = false;
    public bool thisDotMoved = false;
    private Board board;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0f;
    public float swipeResist = 1f;
    public GameObject otherDot;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
       
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f);
        }

        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards Target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
         
        }
        else
        {
            //Directly Set Position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;

        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards Target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);

        }
        else
        {
            //Directly Set Position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
   
        }
    }

    IEnumerator DestroyCo1()
    {
        board.DestroySwipes();        
        yield return new WaitForSeconds(.2f);
        board.RefillSwipes();
    }

    IEnumerator DestroyCo()
    {
        board.DestroyMatches();
        yield return new WaitForSeconds(.2f);
        StartCoroutine(board.FillBoardCo());
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(firstTouchPosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            //Debug.Log(swipeAngle);
            StartCoroutine(DestroyCo1());
            MovePieces();
        }
    }

    void MovePieces()
    {
        
        if (this.gameObject.tag != "Orange Dot" && this.gameObject.tag != "Green Dot" && this.gameObject.tag != "Magenta Dot")
        {
            if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
            {
                //Right Swipe
                otherDot = board.allDots[column + 1, row];
                if (otherDot.tag != "Orange Dot" && otherDot.tag != "Green Dot" && otherDot.tag != "Magenta Dot")
                {
                    otherDot.GetComponent<Dot>().column -= 1;
                    column += 1;
                    this.gameObject.GetComponent<Dot>().thisDotMoved = true;
                    otherDot.GetComponent<Dot>().isMoved = true;
                }

            }
            else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
            {
                //Up Swipe
                otherDot = board.allDots[column, row + 1];
                if (otherDot.tag != "Orange Dot" && otherDot.tag != "Green Dot" && otherDot.tag != "Magenta Dot")
                {
                    otherDot.GetComponent<Dot>().row -= 1;
                    row += 1;
                    this.gameObject.GetComponent<Dot>().thisDotMoved = true;
                    otherDot.GetComponent<Dot>().isMoved = true;
                }

            }
            else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
            {
                //Left Swipe
                otherDot = board.allDots[column - 1, row];
                if (otherDot.tag != "Orange Dot" && otherDot.tag != "Green Dot" && otherDot.tag != "Magenta Dot")
                {
                   otherDot.GetComponent<Dot>().column += 1;
                   column -= 1;
                    this.gameObject.GetComponent<Dot>().thisDotMoved = true;
                   otherDot.GetComponent<Dot>().isMoved = true;
                }

            }
            else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
            {
                //Down Swipe
                otherDot = board.allDots[column, row - 1];
                if (otherDot.tag != "Orange Dot" && otherDot.tag != "Green Dot" && otherDot.tag != "Magenta Dot")
                {
                   otherDot.GetComponent<Dot>().row += 1;
                    row -= 1;
                    this.gameObject.GetComponent<Dot>().thisDotMoved = true;
                    otherDot.GetComponent<Dot>().isMoved = true;
                }
            }
        }
  
    }       
   

    void FindMatches()
    {
        StartCoroutine(DestroyCo());

        if (column > 0 && column < board.width - 1)
        {           
            GameObject leftDot1 = board.allDots[column - 1, row];
            if (board.mixedDot[board.mixedDot.Count - 2] == board.allDots[column + 1, row])
            {
                if (this.gameObject != null && leftDot1 != null && board.mixedDot[board.mixedDot.Count - 1] != null)
                {
                
                    if (board.mixedDot[board.mixedDot.Count - 2].tag == "Orange Dot")
                    {
                        if ((leftDot1.tag == "Red Dot" && this.gameObject.tag == "Yellow Dot") || (leftDot1.tag == "Yellow Dot" && this.gameObject.tag == "Red Dot"))
                        {
                            leftDot1.GetComponent<Dot>().isMatched = true;
                            isMatched = true;
                            //Debug.Log("Matched!");
                        }
                    }
                    else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Magenta Dot")
                    {
                        if ((leftDot1.tag == "Blue Dot" && this.gameObject.tag == "Red Dot") || (leftDot1.tag == "Red Dot" && this.gameObject.tag == "Blue Dot"))
                        {
                            leftDot1.GetComponent<Dot>().isMatched = true;
                            isMatched = true;
                            //Debug.Log("Matched!");
                        }
                    }
                    else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Green Dot")
                    {
                        if ((leftDot1.tag == "Blue Dot" && this.gameObject.tag == "Yellow Dot") || (leftDot1.tag == "Yellow Dot" && this.gameObject.tag == "Blue Dot"))
                        {
                            leftDot1.GetComponent<Dot>().isMatched = true;
                            isMatched = true;
                            //Debug.Log("Matched!");
                        }
                    }
                }
            }                
        }
        if (column > 0 && column < board.width - 1)
        {
            GameObject rightDot1 = board.allDots[column + 1, row];
            if(board.mixedDot[board.mixedDot.Count - 2] == board.allDots[column - 1, row])
            {
                if (rightDot1 != board.allDots[board.height - 1, board.width - 1])
                {
                    if (this.gameObject != null && rightDot1 != null && board.mixedDot[board.mixedDot.Count - 1] != null)
                    {
                        if (board.mixedDot[board.mixedDot.Count - 2].tag == "Orange Dot")
                        {

                            if ((rightDot1.tag == "Red Dot" && this.gameObject.tag == "Yellow Dot") || (rightDot1.tag == "Yellow Dot" && this.gameObject.tag == "Red Dot"))
                            {
                                rightDot1.GetComponent<Dot>().isMatched = true;
                                isMatched = true;
                                //Debug.Log("Matched!");
                            }
                        }
                        else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Magenta Dot")
                        {
                            if ((rightDot1.tag == "Blue Dot" && this.gameObject.tag == "Red Dot") || (rightDot1.tag == "Red Dot" && this.gameObject.tag == "Blue Dot"))
                            {
                                rightDot1.GetComponent<Dot>().isMatched = true;
                                isMatched = true;
                                //Debug.Log("Matched!");
                            }
                        }
                        else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Green Dot")
                        {
                            if ((rightDot1.tag == "Blue Dot" && this.gameObject.tag == "Yellow Dot") || (rightDot1.tag == "Yellow Dot" && this.gameObject.tag == "Blue Dot"))
                            {
                                rightDot1.GetComponent<Dot>().isMatched = true;
                                isMatched = true;
                                // Debug.Log("Matched!");
                            }
                        }
                    }
                }
            }            
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            if(board.mixedDot[board.mixedDot.Count - 2] == board.allDots[column, row - 1])
            {
                if (upDot1 != board.allDots[board.height - 1, board.width - 1])
                {
                    if (this.gameObject != null && upDot1 != null && board.mixedDot[board.mixedDot.Count - 1] != null)
                    {
                        if (board.mixedDot[board.mixedDot.Count - 2].tag == "Orange Dot")
                        {
                            if ((upDot1.tag == "Red Dot" && this.gameObject.tag == "Yellow Dot") || (upDot1.tag == "Yellow Dot" && this.gameObject.tag == "Red Dot"))
                            {
                                upDot1.GetComponent<Dot>().isMatched = true;
                                isMatched = true;
                                //Debug.Log("Matched!");
                            }
                        }
                        else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Magenta Dot")
                        {
                            if ((upDot1.tag == "Blue Dot" && this.gameObject.tag == "Red Dot") || (upDot1.tag == "Red Dot" && this.gameObject.tag == "Blue Dot"))
                            {
                                upDot1.GetComponent<Dot>().isMatched = true;
                                isMatched = true;
                                //Debug.Log("Matched!");
                            }
                        }
                        else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Green Dot")
                        {
                            if ((upDot1.tag == "Blue Dot" && this.gameObject.tag == "Yellow Dot") || (upDot1.tag == "Yellow Dot" && this.gameObject.tag == "Blue Dot"))
                            {
                                upDot1.GetComponent<Dot>().isMatched = true;
                                isMatched = true;
                                //Debug.Log("Matched!");
                            }
                        }
                    }
                }
            }            
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject downDot1 = board.allDots[column, row - 1];
            if(board.mixedDot[board.mixedDot.Count - 2] == board.allDots[column, row + 1])
            {
                if (this.gameObject != null && downDot1 != null && board.mixedDot[board.mixedDot.Count - 1] != null)
                {
                    if (board.mixedDot[board.mixedDot.Count - 2].tag == "Orange Dot")
                    {
                        if ((downDot1.tag == "Red Dot" && this.gameObject.tag == "Yellow Dot") || (downDot1.tag == "Yellow Dot" && this.gameObject.tag == "Red Dot"))
                        {
                            downDot1.GetComponent<Dot>().isMatched = true;
                            isMatched = true;
                            //Debug.Log("Matched!");
                        }
                    }
                    else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Magenta Dot")
                    {
                        if ((downDot1.tag == "Blue Dot" && this.gameObject.tag == "Red Dot") || (downDot1.tag == "Red Dot" && this.gameObject.tag == "Blue Dot"))
                        {
                            downDot1.GetComponent<Dot>().isMatched = true;
                            isMatched = true;
                            //Debug.Log("Matched!");
                        }
                    }
                    else if (board.mixedDot[board.mixedDot.Count - 1].tag == "Green Dot")
                    {
                        if ((downDot1.tag == "Blue Dot" && this.gameObject.tag == "Yellow Dot") || (downDot1.tag == "Yellow Dot" && this.gameObject.tag == "Blue Dot"))
                        {
                            downDot1.GetComponent<Dot>().isMatched = true;
                            isMatched = true;
                            //Debug.Log("Matched!");
                        }
                    }
                }
            }
        }            
    }

    /*void FindMatches()
    {
        StartCoroutine(DestroyCo());

        if (row > 0 && row < board.height - 1 && column == board.width - 1)
        {
            GameObject upDot = board.allDots[board.width - 1, row + 1];
            GameObject downDot = board.allDots[board.width - 1, row - 1];
            if (this.gameObject != null && upDot != null && downDot != null)
            {
                if (upDot.tag == "Orange Dot")
                {

                    if ((this.gameObject.tag == "Red Dot" && downDot.tag == "Yellow Dot") || (this.gameObject.tag == "Yellow Dot" && downDot.tag == "Red Dot"))
                    {
                        downDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                    }
                }
                else if (upDot.tag == "Magenta Dot")
                {

                    if ((this.gameObject.tag == "Red Dot" && downDot.tag == "Blue Dot") || (this.gameObject.tag == "Blue Dot" && downDot.tag == "Red Dot"))
                    {
                        downDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                    }
                }
                else if (upDot.tag == "Green Dot")
                {

                    if ((this.gameObject.tag == "Blue Dot" && downDot.tag == "Yellow Dot") || (this.gameObject.tag == "Yellow Dot" && downDot.tag == "Blue Dot"))
                    {
                        downDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                    }
                }
            }
        }
        if (column > 0 && column < board.width - 1 && row == 0)
        {
            GameObject leftDot = board.allDots[column - 1, 0];
            GameObject rightDot = board.allDots[column + 1, 0];
            if (this.gameObject != null && rightDot != null && leftDot != null)
            {
                if (rightDot.tag == "Orange Dot")
                {
                    if ((this.gameObject.tag == "Red Dot" && leftDot.tag == "Yellow Dot") || (this.gameObject.tag == "Yellow Dot" && leftDot.tag == "Red Dot"))
                    {
                        leftDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                        Debug.Log("matched");
                    }
                }
                else if (rightDot.tag == "Magenta Dot")
                {

                    if ((this.gameObject.tag == "Red Dot" && leftDot.tag == "Blue Dot") || (this.gameObject.tag == "Blue Dot" && leftDot.tag == "Red Dot"))
                    {
                        leftDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                        Debug.Log("matched");
                    }
                }
                else if (rightDot.tag == "Green Dot")
                {

                    if ((this.gameObject.tag == "Blue Dot" && leftDot.tag == "Yellow Dot") || (this.gameObject.tag == "Yellow Dot" && leftDot.tag == "Blue Dot"))
                    {
                        leftDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                        Debug.Log("matched");
                    }
                }
            }
        }
        if (row > 0 && row < board.height - 1 && column == board.width - 2)
        {
            GameObject upDot = board.allDots[board.width - 2, row + 1];
            GameObject downDot = board.allDots[board.width - 2, row - 1];
            if (this.gameObject != null && upDot != null && downDot != null)
            {
                if (downDot.tag == "Orange Dot")
                {
                    if ((this.gameObject.tag == "Red Dot" && upDot.tag == "Yellow Dot") || (this.gameObject.tag == "Yellow Dot" && upDot.tag == "Red Dot"))
                    {
                        upDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                    }
                }
                else if (downDot.tag == "Magenta Dot")
                {

                    if ((this.gameObject.tag == "Red Dot" && upDot.tag == "Blue Dot") || (this.gameObject.tag == "Blue Dot" && upDot.tag == "Red Dot"))
                    {
                        upDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                    }
                }
                else if (downDot.tag == "Green Dot")
                {

                    if ((this.gameObject.tag == "Blue Dot" && upDot.tag == "Yellow Dot") || (this.gameObject.tag == "Yellow Dot" && upDot.tag == "Blue Dot"))
                    {
                        upDot.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                    }
                }
            }
        }
    }*/
}
    

    