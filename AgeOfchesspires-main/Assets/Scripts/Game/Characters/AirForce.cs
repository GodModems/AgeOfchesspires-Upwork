using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirForce : Character
{
    public void AirForceInit()
    {
        value = 7; // todo: need to change
        name = CharacterDef.AIRFORCE;
        imgFileName = "Airforce";
        encode_name = "BA";
        imgFileNameOrig = "AirforceOrig";
    }

    public override List<Char_Point> availableMovePoints(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
{
    List<Char_Point> avalable_move = new List<Char_Point>();
    Char_Point candidate = new Char_Point(-1, -1); 
    Char_Point tmpPos = pos;
    


        

        if (isPromoted || promotedOption > 0)
        {
            int option = promotedOption;

            if (option == 5)//Pawn when converted by Witch
            {
                CoastGuard coastg = new();
                coastg.setIsFirstMove(false);
                coastg.setPosition(getPosition());

                avalable_move = coastg.availableMovePoints(myPos, opponentPos, projectileList);
            }
            

        }
        else
        { 

       
    

    

    

    while (tmpPos.y <= Character.BOARD_HEIGHT /*&& posInQuare != 0*/) // posInQuare = 0 is ? . skip check head + right
    {
        if (CheckIfEmptyAheadRight(tmpPos, myPos, opponentPos, projectileList, ref candidate) )
        {
                
                    avalable_move.Add(candidate);
                    tmpPos = candidate;
                
        }
        else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) )
        {
            avalable_move.Add(candidate);
            break;
        }
        else
        {
            break;
        }
        
    }

    tmpPos = pos;
    while (tmpPos.y >= 0 /*&& posInQuare != 2*/) // posInQuare = 2 is ? . skip check behind + left
    {
        if (CheckIfEmptyBehindLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate) )
        {
                
                    avalable_move.Add(candidate);
                    tmpPos = candidate;
                
        }
        else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) )
        {
            avalable_move.Add(candidate);
            break;
        }
        else
        {
            break;
        }
       
    }

    tmpPos = pos;
    while (tmpPos.y >= 0 /*&& posInQuare != 3*/) // posInQuare = 3 is ?  . skip check behind + right
    {
        if (CheckIfEmptyBehindRight(tmpPos, myPos, opponentPos, projectileList, ref candidate) )
        {
                
                    avalable_move.Add(candidate);
                    tmpPos = candidate;
                
        }
        else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) )
        {
            avalable_move.Add(candidate);
            break;
        }
        else
        {
            break;
        }
        
    }

    tmpPos = pos;
            while (tmpPos.y <= Character.BOARD_HEIGHT /*&& posInQuare != 1*/) // posInQuare = 1 is ? . skip check ahead + left 
            {
                if (CheckIfEmptyAheadLeft(tmpPos, myPos, opponentPos, projectileList, ref candidate))
                {

                    avalable_move.Add(candidate);
                    tmpPos = candidate;

                }
                else if (IsOpponentPlayer(candidate, myPos, opponentPos, projectileList))
                {
                    avalable_move.Add(candidate);
                    break;
                }
                else
                {
                    break;
                }
                
            }
    }

    return avalable_move;
}

    public static bool IsProjectile(List<Char_Point> projectileList, Char_Point p)
    {
        return ContainsCustom(projectileList, p);
    }

    public override List<Char_Point> pointCanKillOpponent(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
{
    List<Char_Point> can_kill = new List<Char_Point>();
    Char_Point candidate = new Char_Point(-1,-1);
    Char_Point tmpPos = pos;
    tmpPos = pos;

        if (isPromoted || promotedOption > 0)
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

           

           

           

           

            tmpPos = pos;
    while (tmpPos.y <= Character.BOARD_HEIGHT /*&& posInQuare != 0*/) // posInQuare = 0 is ? . skip check head + right
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

    tmpPos = pos;
    while (tmpPos.y >= 0 /*&& posInQuare != 2*/) // posInQuare = 2 is ? . skip check behind + left
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
    while (tmpPos.y >= 0 /*&& posInQuare != 3*/) // posInQuare = 3 is ?  . skip check hbehind + right
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
            while (tmpPos.y <= Character.BOARD_HEIGHT /*&& posInQuare != 1*/) // posInQuare = 1 is ? . skip check ahead + left 
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
    }

    return can_kill;
}


    
   
}
