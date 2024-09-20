using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class President : Character
{
    public void PresidentInit()
    {
        value = 20;
        name = CharacterDef.PRESIDENT;
        imgFileName = "President";
        encode_name = "KP";
        imgFileNameOrig = "PresidentOrig";
    }

    public List<Char_Point> CastleCheck()
    {
        List<Char_Point> avalable_move = new List<Char_Point>();
        return avalable_move;
    }


    public override List<Char_Point> availableMovePoints(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        List<Char_Point> available_move = new List<Char_Point>();
        Char_Point candidate = new Char_Point(-1, -1);
        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;



        if (isFirstMove)
            available_move = CastleCheck();

        


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

        bool emptyBehindLeft = false;
        if (CheckIfEmptyBehindLeft(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
        {

            available_move.Add(candidate);
            emptyBehindLeft = true;

            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                if (emptyBehindLeft)
                {
                    Char_Point candidate2 = new Char_Point(-1, -1);

                    if (CheckIfEmptyBehindLeft(candidate, myPos, opponentPos, projectileList, ref candidate2) || IsOpponentPlayer(candidate2, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                    {
                        available_move.Add(candidate2);
                    }
                }
            }

        }


        

        bool emptyBehindRight = false;
        if (CheckIfEmptyBehindRight(pos, myPos, opponentPos, projectileList,ref  candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
        {
            available_move.Add(candidate);
            emptyBehindRight = true;

            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                if (emptyBehindRight)
                {
                    Char_Point candidate2 = new Char_Point(+1, -1);

                    if (CheckIfEmptyBehindRight(candidate, myPos, opponentPos, projectileList, ref candidate2) || IsOpponentPlayer(candidate2, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                    {
                        available_move.Add(candidate2);
                    }
                }
            }
        }

        

        bool emptyAheadLeft = false;
        if (CheckIfEmptyAheadLeft(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
        {
            available_move.Add(candidate);
            emptyAheadLeft = true;

            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                if (emptyAheadLeft)
                {
                    Char_Point candidate2 = new Char_Point(-1, +1);

                    if (CheckIfEmptyAheadLeft(candidate, myPos, opponentPos, projectileList, ref candidate2) || IsOpponentPlayer(candidate2, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                    {
                        available_move.Add(candidate2);
                    }
                }
            }
        }

        

        bool emptyAheadRight = false;
        if (CheckIfEmptyAheadRight(pos, myPos, opponentPos, projectileList, ref candidate) || IsOpponentPlayer(candidate, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
        {
            available_move.Add(candidate);
            emptyAheadRight = true;

            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                if (emptyAheadRight)
                {
                    Char_Point candidate2 = new Char_Point(+1, +1);

                    if (CheckIfEmptyAheadRight(candidate, myPos, opponentPos, projectileList, ref candidate2) || IsOpponentPlayer(candidate2, myPos, opponentPos, projectileList) /*&& !IsProjectile(projectileList, candidate)*/)
                    {
                        available_move.Add(candidate2);
                    }
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

        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;

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

        candidate = new Char_Point(pos.x - 1, pos.y - 1, pos.colour);
        if (IsValidPoint(candidate))
        {
            can_kill.Add(candidate);


            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                Char_Point candidate2 = new Char_Point(candidate.x - 1, candidate.y - 1, candidate.colour);
                if (IsValidPoint(candidate2))
                {
                    can_kill.Add(candidate2);
                }
            }
        }

        candidate = new Char_Point(pos.x + 1, pos.y - 1, pos.colour);
        if (IsValidPoint(candidate))
        {
            can_kill.Add(candidate);
            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                Char_Point candidate2 = new Char_Point(candidate.x + 1, candidate.y - 1, candidate.colour);
                if (IsValidPoint(candidate2))
                {
                    can_kill.Add(candidate2);
                }
            }
        }

        candidate = new Char_Point(pos.x - 1, pos.y + 1, pos.colour);
        if (IsValidPoint(candidate))
        {
            can_kill.Add(candidate);

            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                Char_Point candidate2 = new Char_Point(candidate.x - 1, candidate.y + 1, candidate.colour);
                if (IsValidPoint(candidate2))
                {
                    can_kill.Add(candidate2);
                }
            }
        }
        candidate = new Char_Point(pos.x + 1, pos.y + 1, pos.colour);
        if (IsValidPoint(candidate))
        {
            can_kill.Add(candidate);

            if (myCiv == (int)GameConstants.Civilisation.Egyptian)
            {
                Char_Point candidate2 = new Char_Point(candidate.x + 1, candidate.y + 1, candidate.colour);
                if (IsValidPoint(candidate2))
                {
                    can_kill.Add(candidate2);
                }
            }
        }

        


        return can_kill;
    }
}
