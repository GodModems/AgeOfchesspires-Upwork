using System.Collections.Generic;
using System.Diagnostics;

public enum colour
{
    WHITE,
    BLACK,
    BOTH
}

public enum CharacterDef
{
    PRESIDENT,
    ARMY,
    AIRFORCE,
    NAVY,
    MARINE,
    COASTGUARD,
    ATOMIC,
    POISON_GAS,
    HYD_NUKE,
    BIO_GAS,
    None
}

public enum State
{
    DIED,
    CAPTURE,
    ALIVE
}

public enum Owner
{
    USER,
    COMPUTER
}

public class Char_Point
{
    public int x;
    public int y;
    public colour colour;

    public Char_Point()
    { }

    public Char_Point(int i, int y)
    {
        this.x = i;
        this.y = y;
    }

    public Char_Point(int i, int y, colour colour)
    {
        this.x = i;
        this.y = y;
        this.colour = colour;
    }

    public Char_Point CopyFrom(in Char_Point a)
    {
        x = a.x;
        y = a.y;
        colour = a.colour;
        return this;
    }

    public static bool operator ==(Char_Point ImpliedObject, in Char_Point rhs)
    {
        return ImpliedObject.x == rhs.x && ImpliedObject.y == rhs.y && ImpliedObject.colour == rhs.colour;
    }

    public static bool operator !=(Char_Point ImpliedObject, in Char_Point rhs)
    {
        return !(ImpliedObject == rhs);
    }

    public int index()
    {
        return x + y * 8;
    }
}


public abstract class Character
{
    protected Char_Point pos = new Char_Point();
    protected int value;
    protected new CharacterDef name;//hiding the parent's
    protected string imgFileName = "";
    protected string imgFileNameOrig = "";
    protected Owner owner;
    protected bool isFirstMove = true;
    protected State alive = State.ALIVE;
    protected string encode_name = "";
    protected bool outByComp2 = false;
    protected bool outByComp3 = false;
    public Char_Point initPos;
    public static int BOARD_HEIGHT=7;
    public static int BOARD_WIDTH=7;
    protected bool isPromoted = false;
    protected int promotedOption;
    protected bool isCaptured = false;

    public virtual void Dispose()
    {
    }
    public void setPosition(Char_Point value)
    {
        Debug.WriteLine("CHARACTER::set new position:"+value.x+","+value.y+","+value.colour);
        pos = value;
    }

    public void SetPromoted(bool pPromoted)
    {
        isPromoted = pPromoted;
    }

    public bool GetIsPromoted()
    {
        return isPromoted;
    }
    public void SetPromotedOption(int promotOption)
    {
       promotedOption = promotOption;
    }

    public int GetPromotedOption()
    {
        return promotedOption;
    }

    public Char_Point getPosition()
    {
        return pos;
    }
    public string getEncodeName()
    {
        return encode_name;
    }
    public int getValue()
    {
        return value;
    }
    public void setOwner(Owner x)
    {
        owner= x;
    }
    public Owner getOwner()
    {
        return owner;
    }
    public CharacterDef getName()
    {
        return name;
    }
    public void setIsFirstMove(bool x)
    {
        isFirstMove = x;
    }
    public bool getIsFirstMove()
    {
        return isFirstMove;
    }
    public string getImageName()
    {
        return imgFileName;
    }

    public string getImageNameOrig()
    {
        return imgFileNameOrig;
    }

    public void SetAlive(State x)
    {
        alive = x;
    }
    public State isAlive()
    {
        return alive;
    }

    public bool isOutByComp2()
    {
        return outByComp2;
    }

    public bool isOutByComp3()
    {
        return outByComp3;
    }

    public void setIsOutByComp2(bool x)
    {
        outByComp2 = x;
    }

    public void setIsOutByComp3(bool x)
    {
        outByComp3 = x;
    }

    public void SetIsCaptured(bool capt)
    {
        isCaptured = capt;
    }

    public bool GetIsCaptured()
    {
        return isCaptured;
    }

    public bool isChip()
    {
        return name == CharacterDef.ATOMIC || name == CharacterDef.POISON_GAS || name == CharacterDef.HYD_NUKE || name == CharacterDef.BIO_GAS;
    }

    public virtual Char_Point getInitPos()
    {
        return pos;
    }
    public abstract List<Char_Point> availableMovePoints(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList);

    public virtual List<Char_Point> pointCanKillOpponent(List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        return new List<Char_Point>(availableMovePoints(new List<Char_Point>(myPos), new List<Char_Point>(opponentPos), new List<Char_Point>(projectileList)));
    }

    public virtual void ResetInitPositionTo(Char_Point char_Point)
    {
        initPos = char_Point;
    }


    public static bool CheckIfEmptySpace(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        bool isMyPos=true, isMyOpp=true, isProjLst=true, isMyFrnd = true;
        isMyPos = ContainsCustom(myPos, p);//myPos.Contains(p);
        isMyOpp = ContainsCustom(opponentPos, p);//opponentPos.Contains(p);
        isProjLst = ContainsCustom(projectileList, p);//projectileList.Contains(p);
        isMyFrnd = ContainsCustom(myPos, p);

        if ( isMyPos || isMyOpp || isProjLst || isMyFrnd)
        {
            return false;
        }

        return true;
    }

    public static bool ContainsCustom(List<Char_Point> list, Char_Point point)
    {
        bool isContain = false;

        if(list == null)
        {
            return false;
        }

        foreach(Char_Point char_Point in list)
        {
            if(char_Point.x == point.x && char_Point.y == point.y && char_Point.colour == point.colour)
            {
                isContain = true;
                break;
            }
        }

        



        return isContain;
    }

    public static bool CheckIfCanMoveTo(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList)
    {
        if (p.colour == colour.BOTH)
        {
            Char_Point black = new Char_Point { x = p.x, y = p.y, colour = colour.BLACK };
            Char_Point white = new Char_Point { x = p.x, y = p.y, colour = colour.WHITE };
            if (CheckIfEmptySpace(p, myPos, opponentPos, projectileList) &&
                CheckIfEmptySpace(black, myPos, opponentPos, projectileList) &&
                CheckIfEmptySpace(white, myPos, opponentPos, projectileList))
            {
                Debug.WriteLine("CHARACTER::CheckIfCanMoveTo::Is Empty Space:TRUE");


                return true;
            }
        }
        else
        {
            Char_Point both = new Char_Point { x = p.x, y = p.y, colour = colour.BOTH };
            if (CheckIfEmptySpace(p, myPos, opponentPos, projectileList) &&
                CheckIfEmptySpace(both, myPos, opponentPos, projectileList))
            {
                return true;
            }
        }
        return false;
    }

    public static bool CheckIfEmptyLeft(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyLeft");


        Char_Point left = new Char_Point { x = p.x - 1, y = p.y, colour = p.colour };
        result = left;
        if (!IsValidPoint(left))
            return false;

        Debug.WriteLine("CHARACTER::CheckIfEmptyLeft:: Is Point Valid TRUE");


        if (CheckIfCanMoveTo(left, myPos, opponentPos, projectileList))
            return true;
        return false;
    }

    public static bool CheckIfEmptyRight(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyRight");


        Char_Point sameColour = new Char_Point { x = p.x + 1, y = p.y, colour = p.colour };
        result = sameColour;
        if (!IsValidPoint(sameColour))
            return false;

        Debug.WriteLine("CHARACTER::CheckIfEmptyRight:: Is Point Valid TRUE");


        if (CheckIfCanMoveTo(sameColour, myPos, opponentPos, projectileList))
            return true;
        return false;
    }

    public static bool CheckIfEmptyAhead(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyAhead");


        Char_Point sameColour = new Char_Point { x = p.x, y = p.y + 1, colour = p.colour };
        result = sameColour;
        if (!IsValidPoint(sameColour))
            return false;

        Debug.WriteLine("CHARACTER::CheckIfEmptyAhead:: Is Point Valid TRUE");


        if (CheckIfCanMoveTo(sameColour, myPos, opponentPos, projectileList))
            return true;
        return false;

        
    }

    public static bool CheckIfEmptyAheadFirstMove(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadFirstMove");


        Char_Point sameColour = new Char_Point { x = p.x, y = p.y + 1, colour = p.colour };
        result = sameColour;
        if (!IsValidPoint(sameColour))
            return false;

        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadFirstMove:: Is Point Valid TRUE");


        if (CheckIfCanMoveTo(sameColour, myPos, opponentPos, projectileList))
            return true;
        return false;


    }

    public static bool CheckIfEmptyAheadFirstMove2Steps(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadFirstMove2Steps");


        Char_Point sameColour = new Char_Point { x = p.x, y = p.y + 2, colour = p.colour };
        result = sameColour;
        if (!IsValidPoint(sameColour))
            return false;

        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadFirstMove2Steps:: Is Point Valid TRUE");


        if (CheckIfCanMoveTo(sameColour, myPos, opponentPos, projectileList))
            return true;
        return false;


    }

    public static bool CheckIfEmptyBehind(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyBehind");


        Char_Point sameColour = new Char_Point { x = p.x, y = p.y - 1, colour = p.colour };
        result = sameColour;
        if (!IsValidPoint(sameColour))
            return false;

        Debug.WriteLine("CHARACTER::CheckIfEmptyBehind:: Is Point Valid TRUE");


        if (CheckIfCanMoveTo(sameColour, myPos, opponentPos, projectileList))
            return true;
        return false;
    }

    public static bool CheckIfEmptyBehindLeft(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList,ref  Char_Point result)
    {

        Debug.WriteLine("CHARACTER::CheckIfEmptyBehindLeft");


        Char_Point behind = new Char_Point { x = p.x, y = p.y - 1, colour = p.colour };
        if (!IsValidPoint(behind))
        {
            return false;
        }

        Debug.WriteLine("CHARACTER::CheckIfEmptyBehindLeft:: Is Point Valid TRUE");


        return CheckIfEmptyLeft(
            behind, myPos, opponentPos, projectileList, ref result);
    }

    public static bool CheckIfEmptyBehindRight(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyBehindRight");


        Char_Point behind = new Char_Point { x = p.x, y = p.y - 1, colour = p.colour };
        if (!IsValidPoint(behind))
        {
            return false;
        }

        Debug.WriteLine("CHARACTER::CheckIfEmptyBehindRight:: Is Point Valid TRUE");


        return CheckIfEmptyRight(behind, myPos, opponentPos, projectileList, ref result);
    }

    public static bool CheckIfEmptyAheadLeft(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadLeft");


        Char_Point ahead = new Char_Point { x = p.x, y = p.y + 1, colour = p.colour };
        if (!IsValidPoint(ahead))
        {
            return false;
        }

        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadLeft:: Is Point Valid TRUE");


        return CheckIfEmptyLeft(ahead, myPos, opponentPos, projectileList, ref result);
    }

    public static bool CheckIfEmptyAheadRight(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadRight");


        Char_Point ahead = new Char_Point { x = p.x, y = p.y + 1, colour = p.colour };
        if (!IsValidPoint(ahead))
        {
            return false;
        }

        Debug.WriteLine("CHARACTER::CheckIfEmptyAheadRight:: Is Point Valid TRUE");


        return CheckIfEmptyRight(ahead, myPos, opponentPos, projectileList, ref result);
    }

    //For Egyptians King
    public static bool CheckIfEmptyBehindLeft2Steps(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {


        Char_Point behind = new Char_Point { x = p.x - 1, y = p.y - 1, colour = p.colour };
        if (!IsValidPoint(behind))
        {
            return false;
        }



        return CheckIfEmptyBehindLeft(
            behind, myPos, opponentPos, projectileList, ref result);
    }

    public static bool CheckIfEmptyBehindRight2Steps(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {


        Char_Point behind = new Char_Point { x = p.x + 1, y = p.y - 1, colour = p.colour };
        if (!IsValidPoint(behind))
        {
            return false;
        }

        return CheckIfEmptyBehindRight(behind, myPos, opponentPos, projectileList, ref result);
    }

    public static bool CheckIfEmptyAheadLeft2Steps(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Char_Point ahead = new Char_Point { x = p.x-1, y = p.y + 1, colour = p.colour };
        if (!IsValidPoint(ahead))
        {
            return false;
        }

        return CheckIfEmptyAheadLeft(ahead, myPos, opponentPos, projectileList, ref result);
    }

    public static bool CheckIfEmptyAheadRight2Steps(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList, ref Char_Point result)
    {
        Char_Point ahead = new Char_Point { x = p.x+1, y = p.y + 1, colour = p.colour };
        if (!IsValidPoint(ahead))
        {
            return false;
        }

        return CheckIfEmptyBehindRight(ahead, myPos, opponentPos, projectileList, ref result);
    }
    /// <summary>

    /// </summary>
    /// <param name="p"></param>
    /// <param name="myPos"></param>
    /// <param name="opponentPos"></param>
    /// <param name="projectileList"></param>
    /// <returns></returns>

    //public static bool isProjectile = false;
    public static bool IsOpponentPlayer(Char_Point p, List<Char_Point> myPos, List<Char_Point> opponentPos, List<Char_Point> projectileList = null)
    {

        Debug.WriteLine("CHARACTER::IsOpponentPlayer?");


        Char_Point full_space = new Char_Point { x = p.x, y = p.y, colour = colour.BOTH };

        if (ContainsCustom(opponentPos, full_space))
        {
            Debug.WriteLine("CHARACTER::ContainsCustom::TRUE");

            return true;
        }

        if (p.colour == colour.BOTH)
        {
            Char_Point black_point = new Char_Point { x = p.x, y = p.y, colour = colour.BLACK };
            Char_Point white_point = new Char_Point { x = p.x, y = p.y, colour = colour.WHITE };

            bool containsBlackOpp = ContainsCustom(opponentPos, black_point);
            bool containsWhiteMy = ContainsCustom(myPos, white_point);
            bool containsWhiteProj = ContainsCustom(projectileList, white_point);

            bool containsWhiteOpp = ContainsCustom(opponentPos, white_point);
            bool containsBlackMy = ContainsCustom(myPos, black_point);
            bool containsBlackProj = ContainsCustom(projectileList, black_point);

            if (containsBlackOpp &&
                !containsWhiteMy &&
                !containsWhiteProj)
            {

                Debug.WriteLine("CHARACTER::ContainsCustom BOTH 1::TRUE");


                return true;
            }
            
            if (containsWhiteOpp &&
                !containsBlackMy &&
                !containsBlackProj)
            {
                Debug.WriteLine("CHARACTER::ContainsCustom BOTH 2::TRUE");


                return true;
            }
        }
        else
        {
            Debug.WriteLine("CHARACTER::ContainsCustom ELSE:");

            if (ContainsCustom(opponentPos, p) || ContainsCustom(opponentPos, full_space) )
            {
                Debug.WriteLine("CHARACTER::ContainsCustom TRUE");

                return true;
            }
        }

        Debug.WriteLine("CHARACTER::ContainsCustom FALSE");

        return false;
    }

    public static bool IsProjectile(List<Char_Point> projectileList, Char_Point p)
    {
        return ContainsCustom(projectileList, p);
    }

    public static int GetWhitePosInSquare(Char_Point p, int firstPos = 1)
    {

        int white_pos = 0;
        int black_pos = 0;
        if (p.y % 2 == 0 && p.x % 2 == 0)
        {
            white_pos = firstPos;
        }
        else if (p.y % 2 == 0 && p.x % 2 != 0)
        {
            white_pos = firstPos - 1;
        }
        else if (p.x % 2 == 0)
        {
            white_pos = firstPos == 1 ? 2 : 0;
        }
        else
        {
            white_pos = firstPos == 1 ? 3 : 2;
        }
        black_pos = (white_pos + 2) % 4;
        if (p.colour == colour.WHITE)
            return white_pos;
        return black_pos;
    }

    public static bool IsValidPoint(Char_Point p)
    {
        if (p.x > BOARD_WIDTH || p.x < 0 || p.y > BOARD_HEIGHT || p.y < 0)
            return false;


        Debug.WriteLine("CHARACTER::IsValidPoint TRUE");

        return true;
    }

    public void SetOriginalImage()
    {

    }
}
