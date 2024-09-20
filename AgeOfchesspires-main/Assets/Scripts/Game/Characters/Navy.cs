using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Navy : Character
{
    public void NavyInit()
    {
        value = 5;
        name = CharacterDef.NAVY;
        imgFileName = "Navy";
        encode_name = "KN";
        imgFileNameOrig = "NavyOrig";
    }

    public override List<Char_Point> availableMovePoints(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        MultiplayerGamePlay multiplayerGamePlay = GameObject.FindObjectOfType<MultiplayerGamePlay>();
        int myCiv = multiplayerGamePlay.currMyCiv;

        

        List<Char_Point> available_move = new List<Char_Point>();
       // Char_Point tmpPos, tmpPosBoth;

        if (myCiv == (int)GameConstants.Civilisation.Haitians)
        {
           

            int option = promotedOption;
            if (isPromoted || option > 0)
            {
                
                if (option == 1)//
                {
                    Army army = new();
                    army.setPosition(getPosition());
                    available_move = army.availableMovePoints(myPos, opponentPos, projectileList);
                }
                else if (option == 2)//
                {
                    AirForce airForce = new();
                    airForce.setPosition(getPosition());

                    available_move = airForce.availableMovePoints(myPos, opponentPos, projectileList);
                }
                else if (option == 3)//
                {
                    Navy navy = new();
                    navy.setPosition(getPosition());

                    available_move = navy.availableMovePoints(myPos, opponentPos, projectileList);
                }
                else if (option == 4)//
                {
                    Marine marine = new();
                    marine.setPosition(getPosition());

                    available_move = marine.availableMovePoints(myPos, opponentPos, projectileList);
                }
                else if (option == 5)//
                {
                    CoastGuard coastg = new();
                    coastg.setIsFirstMove(false);
                    coastg.setPosition(getPosition());

                    available_move = coastg.availableMovePoints(myPos, opponentPos, projectileList);
                }


            }
            else
            {
                available_move = AvailableMovePointsOriginal(myPos, opponentPos, projectileList);
            }
        }
        else
        {
            available_move = AvailableMovePointsOriginal(myPos, opponentPos, projectileList);
        }

        

        if(myCiv == (int)GameConstants.Civilisation.Burgendy)
        {
            Char_Point candidate = new Char_Point(-1, -1);

            if (CheckIfEmptyLeft(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
            {
                available_move.Add(candidate);
            }

            if (CheckIfEmptyRight(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
            {
                available_move.Add(candidate);
            }

            if (CheckIfEmptyAhead(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
            {
                available_move.Add(candidate);
            }

            if (CheckIfEmptyBehind(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
            {
                available_move.Add(candidate);
            }
        }


        return available_move;
    }

    private List<Char_Point> AvailableMovePointsOriginal(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        Char_Point tmpPos, tmpPosBoth;
        tmpPos = new Char_Point(pos.x + 2, pos.y + 1, pos.colour);
        tmpPosBoth = new Char_Point(tmpPos.x, tmpPos.y, colour.BOTH);

        List<Char_Point> available_move = new List<Char_Point>();



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

    public static bool IsProjectile(List<Char_Point> projectileList, Char_Point p)
    {
        return ContainsCustom(projectileList, p);
    }
    public override List<Char_Point> pointCanKillOpponent(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        List<Char_Point> can_kill = new List<Char_Point>();
        Char_Point tmpPos;

        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;


        
            if (isPromoted)
            {
                int option = GetPromotedOption();
                if (option == 1)//
                {
                    Army army = new();
                    can_kill = army.pointCanKillOpponent(myPos, opponentPos, projectileList);
                }
                else if (option == 2)//
                {
                    AirForce airForce = new();
                    can_kill = airForce.pointCanKillOpponent(myPos, opponentPos, projectileList);
                }
                else if (option == 3)//
                {
                    Navy navy = new();
                    can_kill = navy.pointCanKillOpponent(myPos, opponentPos, projectileList);
                }
                else if (option == 4)//
                {
                    Marine marine = new();
                    can_kill = marine.pointCanKillOpponent(myPos, opponentPos, projectileList);
                }
                else if (option == 5)//
                {
                    CoastGuard coastg = new();
                    coastg.setIsFirstMove(false);
                    can_kill = coastg.pointCanKillOpponent(myPos, opponentPos, projectileList);
                }


            }
            else
            {
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
            }
        

        

        if (myCiv == (int)GameConstants.Civilisation.Burgendy)
        {
            Char_Point candidate = new Char_Point(-1, -1);
            candidate = new Char_Point(pos.x - 1, pos.y, pos.colour);

            if (IsValidPoint(candidate))
            {
                can_kill.Add(candidate);
            }
            candidate = new Char_Point(pos.x + 1, pos.y, pos.colour);
            if (IsValidPoint(candidate))
            {
                can_kill.Add(candidate);
            }

            candidate = new Char_Point(pos.x, pos.y + 1, pos.colour);
            if (IsValidPoint(candidate))
            {
                can_kill.Add(candidate);
            }

            candidate = new Char_Point(pos.x, pos.y - 1, pos.colour);
            if (IsValidPoint(candidate))
            {
                can_kill.Add(candidate);
            }
        }


        return can_kill;
    }
}
