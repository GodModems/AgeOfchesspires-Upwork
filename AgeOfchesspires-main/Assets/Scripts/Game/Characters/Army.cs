using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Army : Character
{
    public void ArmyInit()
    {
        value = 10; // todo: need to change
        name = CharacterDef.ARMY;
        imgFileName = "Army";
        encode_name = "QA";
        imgFileNameOrig = "ArmyOrig";
    }

    public override List<Char_Point> availableMovePoints(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        List<Char_Point> available_move = new List<Char_Point>();
        Char_Point candidate =  new Char_Point(-1, -1);
        Char_Point tmpPos = pos;

        if (isPromoted || promotedOption > 0)
        {
            int option = promotedOption;
           

            if (option == 5)//Pawn when converted by Witch
            {
                Debug.Log("Army::Available move points option 5");
                CoastGuard coastg = new();
                coastg.setIsFirstMove(false);
                coastg.setPosition(getPosition());

                available_move = coastg.availableMovePoints(myPos, opponentPos, projectileList);
            }


        }
        else
        {

            while (tmpPos.x >= 0)
            {
                if (CheckIfEmptyLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {

                    available_move.Add(candidate);
                    tmpPos = candidate;

                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.x <= Character.BOARD_WIDTH)
            {
                if (CheckIfEmptyRight(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {

                    available_move.Add(candidate);
                    tmpPos = candidate;

                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y <= Character.BOARD_HEIGHT)
            {
                if (CheckIfEmptyAhead(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {

                    available_move.Add(candidate);
                    tmpPos = candidate;

                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y >= 0)
            {
                if (CheckIfEmptyBehind(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {

                    available_move.Add(candidate);
                    tmpPos = candidate;

                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y >= 0)
            {
                if (CheckIfEmptyBehindLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    tmpPos = candidate;
                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList)/* && !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y >= 0)
            {
                if (CheckIfEmptyBehindRight(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    tmpPos = candidate;
                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList)/* && !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y <= Character.BOARD_HEIGHT)
            {
                if (CheckIfEmptyAheadLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    tmpPos = candidate;
                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y <= Character.BOARD_HEIGHT)
            {
                if (CheckIfEmptyAheadRight(tmpPos, myPos, opponentPos, projectileList, ref candidate) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    tmpPos = candidate;
                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                {
                    available_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        return available_move;
    }
    public static bool IsProjectile(List<Char_Point> projectileList, Char_Point p)
    {
        return ContainsCustom(projectileList, p);
    }
    public override List<Char_Point> pointCanKillOpponent(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        List<Char_Point> can_kill = new List<Char_Point>();
        Char_Point candidate = new Char_Point(-1, -1);
        Char_Point tmpPos = pos;

        if (isPromoted || promotedOption >= 0)
        {
            int option = promotedOption;
            if (option == 5)//Pawn when converted by Witch
            {
                CoastGuard coastg = new();
                coastg.setIsFirstMove(false);
                coastg.setPosition(getPosition());

                can_kill = coastg.pointCanKillOpponent(myPos, opponentPos, projectileList);
            }


        }
        else
        {

            while (tmpPos.x >= 0)
            {
                if (CheckIfEmptyLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.x <= Character.BOARD_WIDTH)
            {
                if (CheckIfEmptyRight(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y <= Character.BOARD_HEIGHT)
            {
                if (CheckIfEmptyAhead(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y >= 0)
            {
                if (CheckIfEmptyBehind(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y >= 0)
            {
                if (CheckIfEmptyBehindLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y >= 0)
            {
                if (CheckIfEmptyBehindRight(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y <= Character.BOARD_HEIGHT)
            {
                if (CheckIfEmptyAheadLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }

            tmpPos = pos;
            while (tmpPos.y <= Character.BOARD_HEIGHT)
            {
                if (CheckIfEmptyAheadRight(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    tmpPos = candidate;
                }
                else
                {
                    if (IsValidPoint(candidate))
                        can_kill.Add(candidate);
                    break;
                }
            }
        }
        return can_kill;
    }
}
