using System.Collections.Generic;
//using System.Net.NetworkInformation;
using UnityEngine;

public class CoastGuard : Character
{
    private Char_Point initPos;

    public void CoastGuardInit()
    {
        value = 1;
        name = CharacterDef.COASTGUARD;
        imgFileName = "Coast-Guard";
        encode_name = "PC";
        imgFileNameOrig = "Coast-GuardOrig";
    }

    public void CoastGuardInit(Char_Point initPos)
    {
        value = 1;
        name = CharacterDef.COASTGUARD;
        imgFileName = "Coast-Guard";
        encode_name = "PC";
        this.initPos = initPos;
    }

    public bool goToTheEnd(Char_Point movePos)
    {
        if (initPos.y == 1 && movePos.y == BOARD_HEIGHT && pos.y == (BOARD_HEIGHT - 1))
        {
            return true;
        }
        else if (initPos.y == 6 && movePos.y == 0 && pos.y == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override List<Char_Point> availableMovePoints(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        List<Char_Point> available_move = new List<Char_Point>();
        Char_Point candidate = new Char_Point(-1, -1);

        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;

        if (isPromoted || promotedOption > 0)
        {
            int option = promotedOption;
            if (option == 3)//Knight
            {
                Navy navy = new();
                navy.setPosition(getPosition());

                available_move = navy.availableMovePoints(myPos, opponentPos, projectileList);
                //available_move = availableMovePointsPromoted(myPos, opponentPos, projectileList);
            }
            else if (option == 4)//Tower
            {
                Marine marine = new();
                marine.setPosition(getPosition());

                available_move = marine.availableMovePoints(myPos, opponentPos, projectileList);
            }
            else if (option == 2)//elephant
            {
                AirForce airForce = new();
                airForce.setPosition(getPosition());

                available_move = airForce.availableMovePoints(myPos, opponentPos, projectileList);
            }
            else if (option == 1)//Wazir
            {
                Army army = new();
                army.setPosition(getPosition());

                available_move = army.availableMovePoints(myPos, opponentPos, projectileList);
            }
            else if (option == 10)//Witch
            {
                available_move = availableMovePointsPromoted(myPos, opponentPos, projectileList);
            }

        }
        else
        {

            


            




            if (isFirstMove)
            {
                if (myCiv == (int)GameConstants.Civilisation.French)//French cant move forward
                {
                    Debug.Log("Adding available_move points for French");

                    Char_Point cp = new Char_Point(pos.x - 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }
                }
                else if (myCiv == (int)GameConstants.Civilisation.Huns)//huns 
                {
                    Debug.Log("Adding available_move points for French");

                    Char_Point cp = new Char_Point(pos.x - 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }
                }
                else
                {

                    bool isEmptyAhead = false;
                    if (CheckIfEmptyAheadFirstMove(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, projectileList))
                    {
                        isEmptyAhead = true;
                        if (IsOpponentPlayer(candidate, myPos, projectileList) /*&& IsProjectile(projectileList, candidate)*/)
                        {
                            Char_Point jump_over = new Char_Point(candidate.x, candidate.y + 1, candidate.colour);
                            if (IsValidPoint(jump_over) && (CheckIfCanMoveTo(jump_over, myPos, opponentPos, projectileList) || IsOpponentPlayer(jump_over, myPos, opponentPos, projectileList)))
                            {
                                available_move.Add(jump_over);

                            }
                        }
                        else
                        {
                            //if (!IsOpponentPlayer(candidate, myPos, opponentPos, projectileList))
                            //{
                            available_move.Add(candidate);
                            //}
                        }
                    }

                    if (isEmptyAhead)
                    {
                        if (CheckIfEmptyAheadFirstMove2Steps(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, projectileList))
                        {
                            if (IsOpponentPlayer(candidate, myPos, projectileList) /*&& IsProjectile(projectileList, candidate)*/)
                            {
                                Char_Point jump_over = new Char_Point(candidate.x, candidate.y + 1, candidate.colour);
                                if (IsValidPoint(jump_over) && (CheckIfCanMoveTo(jump_over, myPos, opponentPos, projectileList) || IsOpponentPlayer(jump_over, myPos, opponentPos, projectileList)))
                                {
                                    available_move.Add(jump_over);

                                }
                            }
                            else
                            {
                                //if (!IsOpponentPlayer(candidate, myPos, opponentPos, projectileList))
                                //{
                                available_move.Add(candidate);
                                //}
                            }
                        }
                    }

                }
            }
            else
            {
                if (myCiv == (int)GameConstants.Civilisation.French)
                {
                    Debug.Log("Adding available_move points for French");

                    Char_Point cp = new Char_Point(pos.x - 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }
                }
                else if (myCiv == (int)GameConstants.Civilisation.Huns)//huns 
                {
                    Debug.Log("Adding available_move points for French");

                    Char_Point cp = new Char_Point(pos.x - 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x - 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y - 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }

                    cp = new Char_Point(pos.x + 1, pos.y + 1, pos.colour);
                    if (IsValidPoint(cp) && (CheckIfCanMoveTo(cp, myPos, opponentPos, projectileList) || IsOpponentPlayer(cp, myPos, opponentPos, projectileList)))
                    {
                        available_move.Add(cp);
                    }
                }
                else
                {
                    Debug.Log("CG 1");
                    if (CheckIfEmptyAhead(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, projectileList))
                    {
                        Debug.Log("CG 2");

                        if (IsOpponentPlayer(candidate, myPos, projectileList) /*&& IsProjectile(projectileList, candidate)*/)
                        {
                            Debug.Log("CG 3");

                            Char_Point jump_over = new Char_Point(candidate.x, candidate.y + 1, candidate.colour);
                            if (IsValidPoint(jump_over) && (CheckIfCanMoveTo(jump_over, myPos, opponentPos, projectileList) || IsOpponentPlayer(jump_over, myPos, opponentPos, projectileList)))
                            {
                                Debug.Log("CG 4");

                                available_move.Add(jump_over);

                            }
                        }
                        else
                        {
                            Debug.Log("CG 5");

                            //if (!IsOpponentPlayer(candidate, myPos, opponentPos, projectileList))
                            //{
                            available_move.Add(candidate);
                            //}
                        }
                    }




                }
            }
        }

       

        List<Char_Point> can_captures =
            new List<Char_Point>();
        if (myCiv != (int)GameConstants.Civilisation.Burgendy || !isPromoted)
        {

            can_captures.Add(new Char_Point(pos.x - 1, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y + 1, pos.colour));

               

        }

        

        Debug.Log("My Civ:Coast Guard::" + myCiv);
        if (myCiv == (int)GameConstants.Civilisation.Vikings)//vikings
        {
            Debug.Log("Adding  points for Viking");
            can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));

        }
        else if (myCiv == (int)GameConstants.Civilisation.Britons)//briton
        {
            Debug.Log("Adding  points for Briton");
            can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y + 2, pos.colour));

        }
        else if (myCiv == (int)GameConstants.Civilisation.French)//
        {
            Debug.Log("Adding  points for French");
            can_captures.Add(new Char_Point(pos.x - 1, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y + 1, pos.colour));
        }
        else if (myCiv == (int)GameConstants.Civilisation.Huns)//
        {
            Debug.Log("Adding  points for huns");
            can_captures.Add(new Char_Point(pos.x - 1, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));

            can_captures.Add(new Char_Point(pos.x + 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y + 1, pos.colour));
        }
        else if (myCiv == (int)GameConstants.Civilisation.Burgendy)//
        {
            Debug.Log(":::::Adding Capture points for Burgendy");

            if (isPromoted || promotedOption >= 0)
            {
                if (promotedOption == 3)
                {
                    Debug.Log(":::::Promoted!!!");

                    can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));
                    can_captures.Add(new Char_Point(pos.x, pos.y - 1, pos.colour));
                    can_captures.Add(new Char_Point(pos.x - 1, pos.y, pos.colour));
                    can_captures.Add(new Char_Point(pos.x + 1, pos.y, pos.colour));

                    can_captures.Add(new Char_Point(pos.x + 1, pos.y + 2, pos.colour));
                    can_captures.Add(new Char_Point(pos.x - 1, pos.y + 2, pos.colour));
                    can_captures.Add(new Char_Point(pos.x - 2, pos.y + 1, pos.colour));
                    can_captures.Add(new Char_Point(pos.x + 2, pos.y + 1, pos.colour));
                    can_captures.Add(new Char_Point(pos.x - 2, pos.y - 1, pos.colour));
                    can_captures.Add(new Char_Point(pos.x + 2, pos.y - 1, pos.colour));
                    can_captures.Add(new Char_Point(pos.x - 1, pos.y - 2, pos.colour));
                    can_captures.Add(new Char_Point(pos.x + 1, pos.y - 2, pos.colour));
                }
            }
        }

       

        foreach (Char_Point check_point in can_captures)
        {
            //isProjectile = false;

            if (!IsValidPoint(check_point))
                continue;

            if (IsOpponentPlayer(check_point, myPos, opponentPos, projectileList) )
            {
                
                    available_move.Add(check_point); //capture
                
            }
            else
            {
                if (CheckIfCanMoveTo(check_point, myPos, opponentPos, projectileList) &&
                    (IsOpponentPlayer(new Char_Point(pos.x, check_point.y, pos.colour), myPos, projectileList) ||
                     IsOpponentPlayer(new Char_Point(check_point.x, pos.y, pos.colour), myPos, projectileList)))
                {
                    
                        available_move.Add(check_point); // remove projectile
                    
                }
            }
        }

        return available_move;
    }

    public override List<Char_Point> pointCanKillOpponent(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        List<Char_Point> can_kill = new List<Char_Point>();

        List<Char_Point> can_captures = new List<Char_Point>();

        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;
        Debug.Log("::::My Civ:Coast Guard::" + myCiv);


        if (myCiv == (int)GameConstants.Civilisation.Burgendy && isPromoted || promotedOption == 3)
        {
            
            can_captures = pointCanKillOpponentPromoted(myPos, opponentPos, projectileList);
           
        }
        else
        {
            can_captures.Add(new Char_Point(pos.x - 1, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y - 1, pos.colour));
        }
        /*if (isFirstMove)
        {
            can_captures.Add(new Char_Point(pos.x, pos.y + 2, pos.colour));
        }*/

        

        if(myCiv == (int)GameConstants.Civilisation.Vikings)//vikings
        {
            Debug.Log("Adding Kill points for Viking");
            can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));

        }
        else if (myCiv == (int)GameConstants.Civilisation.Britons)//briton
        {
            Debug.Log("Adding Kill points for Briton");
            can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y + 2, pos.colour));

        }
        else if (myCiv == (int)GameConstants.Civilisation.French)//
        {
            Debug.Log("Adding Kill points for French");
            can_captures.Add(new Char_Point(pos.x - 1, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y + 1, pos.colour));
        }
        else if (myCiv == (int)GameConstants.Civilisation.Huns)//
        {
            Debug.Log("Adding Kill points for French");
            can_captures.Add(new Char_Point(pos.x - 1, pos.y + 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x - 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));

            can_captures.Add(new Char_Point(pos.x + 1, pos.y - 1, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y, pos.colour));
            can_captures.Add(new Char_Point(pos.x + 1, pos.y + 1, pos.colour));
        }
        else if (myCiv == (int)GameConstants.Civilisation.Burgendy)//
        {
            Debug.Log("Adding Kill points for Burgendy ");
            if (isPromoted && GetPromotedOption() == 3)//Knight
            {
                Debug.Log("Promoted ....");
                can_captures.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));
                can_captures.Add(new Char_Point(pos.x, pos.y - 1, pos.colour));
                can_captures.Add(new Char_Point(pos.x - 1, pos.y, pos.colour));
                can_captures.Add(new Char_Point(pos.x + 1, pos.y, pos.colour));


            }
        }

        if (isPromoted || promotedOption > 0)
        {
            int option = GetPromotedOption();
            if (option == 1)//Tower
            {
                Marine marine = new();
                can_captures = marine.pointCanKillOpponent(myPos, opponentPos, projectileList);
            }
            else if (option == 2)//elephant
            {
                AirForce airForce = new();
                can_captures = airForce.pointCanKillOpponent(myPos, opponentPos, projectileList);
            }
            else if (option == 3)//Wazir
            {
                Army army = new();
                can_captures = army.pointCanKillOpponent(myPos, opponentPos, projectileList);
            }
            else if (option == 5)//Witch/Knight
            {
                can_captures = pointCanKillOpponentPromoted(myPos, opponentPos, projectileList);
            }
        }

        foreach (Char_Point check_point in can_captures)
        {
            if (IsValidPoint(check_point))
            {
                can_kill.Add(check_point); //capture
            }
            else
            {
                Debug.Log(check_point.x+":Point Not Valid:"+check_point.y);
            }
        }
        List<Char_Point> ahead = new List<Char_Point> { new Char_Point(pos.x - 1, pos.y, pos.colour), new Char_Point(pos.x + 1, pos.y, pos.colour), new Char_Point(pos.x, pos.y - 1, pos.colour), new Char_Point(pos.x, pos.y + 1, pos.colour) };
        List<Char_Point> jump_over = new List<Char_Point> { new Char_Point(pos.x - 2, pos.y, pos.colour), new Char_Point(pos.x + 2, pos.y, pos.colour), new Char_Point(pos.x, pos.y - 2, pos.colour), new Char_Point(pos.x, pos.y + 2, pos.colour) };
        for (int i = 0; i < ahead.Count; i++)
        {
            if (IsOpponentPlayer(ahead[i], myPos, projectileList) && IsValidPoint(jump_over[i]))
            {
                can_kill.Add(jump_over[i]);
            }
           
        }

        Debug.Log("Kill Expected Count::"+can_kill.Count);
        return can_kill;
    }

    //After promotion
    public List<Char_Point> availableMovePointsPromoted(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        Debug.Log("::::available move points promoted Promoted");

        List<Char_Point> available_move = new List<Char_Point>();
        Char_Point tmpPos, tmpPosBoth;

        if (IsValidPoint(new Char_Point(pos.x, pos.y + 1, pos.colour)))
            available_move.Add(new Char_Point(pos.x, pos.y + 1, pos.colour));
        
        if (IsValidPoint(new Char_Point(pos.x, pos.y - 1, pos.colour)))
            available_move.Add(new Char_Point(pos.x, pos.y - 1, pos.colour));

        if (IsValidPoint(new Char_Point(pos.x - 1, pos.y, pos.colour)))
            available_move.Add(new Char_Point(pos.x - 1, pos.y, pos.colour));

        if (IsValidPoint(new Char_Point(pos.x + 1, pos.y, pos.colour)))
            available_move.Add(new Char_Point(pos.x + 1, pos.y, pos.colour));

        tmpPos = new Char_Point(pos.x + 2, pos.y + 1, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
           ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
               || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{ 
            available_move.Add(tmpPos);
            //}
        }
        tmpPos = new Char_Point(pos.x + 2, pos.y - 1, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
            ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
            || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{ 
            available_move.Add(tmpPos);
            //}
        }
        tmpPos = new Char_Point(pos.x - 2, pos.y + 1, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
            ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
                || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{ 
            available_move.Add(tmpPos);
            //}
        }
        tmpPos = new Char_Point(pos.x - 2, pos.y - 1, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
            ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
                || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{ 
            available_move.Add(tmpPos);
            //}
        }
        tmpPos = new Char_Point(pos.x + 1, pos.y + 2, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
            ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
                || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{ 
            available_move.Add(tmpPos);
            //}
        }
        tmpPos = new Char_Point(pos.x + 1, pos.y - 2, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
            ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
                || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{
            available_move.Add(tmpPos);
            //}
        }
        tmpPos = new Char_Point(pos.x - 1, pos.y + 2, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
            ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
                || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{
            available_move.Add(tmpPos);
            //}
        }
        tmpPos = new Char_Point(pos.x - 1, pos.y - 2, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);
        if (IsValidPoint(tmpPos) &&
            ((CheckIfEmptySpace(tmpPos, myPos, opponentPos, projectileList) && CheckIfEmptySpace(tmpPosBoth, myPos, opponentPos, projectileList))
                || IsOpponentPlayer(tmpPos, myPos, opponentPos)))
        {
            //if (!IsProjectile(projectileList, tmpPos) && !IsProjectile(projectileList, tmpPosBoth))
            //{
            available_move.Add(tmpPos);
            //}
        }
        return available_move;
    }
    
    public List<Char_Point> pointCanKillOpponentPromoted(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        Debug.Log("::::Point Can Kil OPP Promoted");

        List<Char_Point> can_kill = new List<Char_Point>();
        Char_Point tmpPos;

        tmpPos = new Char_Point(pos.x + 2, pos.y + 1, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        tmpPos = new Char_Point(pos.x + 2, pos.y - 1, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        tmpPos = new Char_Point(pos.x - 2, pos.y + 1, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        tmpPos = new Char_Point(pos.x - 2, pos.y - 1, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        tmpPos = new Char_Point(pos.x + 1, pos.y + 2, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        tmpPos = new Char_Point(pos.x + 1, pos.y - 2, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        tmpPos = new Char_Point(pos.x - 1, pos.y + 2, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        tmpPos = new Char_Point(pos.x - 1, pos.y - 2, pos.colour);
        if (IsValidPoint(tmpPos))
            can_kill.Add(tmpPos);

        return can_kill;
    }
}