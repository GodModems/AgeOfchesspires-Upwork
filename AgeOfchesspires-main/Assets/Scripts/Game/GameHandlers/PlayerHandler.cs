using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { ONGOING, USER_WIN, COMP_WIN }
public enum TurnState { PLAYER_TURN, OPP1_BOT_TURN, OPP2_BOT_TURN, FRND_BOT_TURN };


public class moveStatus
{
    public Character player;
    public Char_Point pos;
    public bool is_first_move;
    public State isAlive;

    public moveStatus() { }

    public moveStatus(Character player, Char_Point pos, bool is_first_move, State isAlive)
    {
        this.player = player;
        this.pos = pos;
        this.is_first_move = is_first_move;
        this.isAlive = isAlive;
    }

    public void reverse_status()
    {
        player.setPosition(pos);
        player.setIsFirstMove(is_first_move);
        player.SetAlive(isAlive);
    }
}


public class moveDecission
{
    public Character player;
    public bool is_chip;
    public Char_Point moveTo = new Char_Point(-1,-1, colour.BOTH);
    public List<Char_Point> available_move = new List<Char_Point>();
}

public class indexPlayer
{
    public int who; //  0 Computer, 1 User, -1 invalid
    public int index;
}

public class updatePosStatus
{
    public bool isValid = true;
    public List<int> removePlayers = new List<int>();
}

public class PlayerHandler : MonoBehaviour
{
    private static PlayerHandler pInstance;
    private List<Char_Point> userPlayersPosition = new List<Char_Point>();
    private List<Char_Point> computerPlayersPosition = new List<Char_Point>();
    private List<Char_Point> projectitlePosition = new List<Char_Point>();
    private List<Character> userPlayers = new List<Character>();
    private List<Character> computerPlayers = new List<Character>();
    private List<Character> userChips = new List<Character>();
    private List<Character> computerChips = new List<Character>();
    private HashSet<int> couldBeKilledByComp = new HashSet<int>();
    private HashSet<int> couldBeKilledByUser = new HashSet<int>();
    //private TurnState _turnState;
    public Stack<TurnState> turnStateSeqnce = new Stack<TurnState>();


    private Character createCharacter(Char_Point p, CharacterDef who, Owner owner)
    {
        Character pPlayer = null;

        switch (who)
        {
            case CharacterDef.PRESIDENT:
                President player1 = new President();
                player1.PresidentInit();
                pPlayer =  player1;
                pPlayer.initPos = p;
                break;
            case CharacterDef.ARMY:
                Army player2 = new Army();
                player2.ArmyInit();
                pPlayer = player2;

                pPlayer.initPos = p;
                break;
            case CharacterDef.AIRFORCE:
                AirForce player3 = new AirForce();
                player3.AirForceInit();
                pPlayer = player3;
                pPlayer.initPos = p;
                break;
            case CharacterDef.NAVY:
                Navy player4 = new Navy();
                player4.NavyInit();
                pPlayer = player4;
                pPlayer.initPos = p;
                break;
            case CharacterDef.MARINE:
                Marine player5 = new Marine();
                player5.MarineInit();
                pPlayer = player5;
                pPlayer.initPos = p;
                break;
            case CharacterDef.COASTGUARD:
                CoastGuard player6 = new CoastGuard();
                player6.CoastGuardInit(p);
                pPlayer = player6;
                pPlayer.initPos = p;
                break;
           
        }

        if (pPlayer != null)
        {
            

            pPlayer.setPosition(p);
            pPlayer.setOwner(owner);
        }

        return pPlayer;

    }
    private Stack<List<moveStatus>> changeTracking = new Stack<List<moveStatus>>();
    private List<string> encode_games = new List<string>();
    private List<Character> castle_move = new List<Character>();
    private bool isPosibleCastle;
    private HashSet<colour> rescuecolour = new HashSet<colour>();
    private List<Char_Point> available_promote_point = new List<Char_Point>();
    private bool isWaitingForPromote;
    private float user_point;
    private float comp_point;
    private float comp2_point;
    private float comp3_point;
    private bool isSwitchPresidentPostion;
    private bool isGameTypeChance;
    private bool isBlack;

    private string encodeCharPoint(Char_Point p, bool is_black = false)
    {
        List<string> column16_ref = new List<string> { "p", "o", "n", "m", "l", "k", "j", "i", "h", "g", "f", "e", "d", "c", "b", "a" };
        List<string> row16_ref = new List<string> { "16", "15", "14", "13", "12", "11", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" };
        List<string> column8_ref = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
        List<string> row8_ref = new List<string> { "I", "II", "III", "IV", "V", "VI", "VII", "VIII" };

        List<string> row16 = new List<string>(row16_ref);
        List<string> column16 = new List<string>(column16_ref);
        List<string> row8 = new List<string>(row8_ref);
        List<string> column8 = new List<string>(column8_ref);

        

      

        if (p.x < 0 || p.x > 7 || p.y < 0 || p.y > 7)
            return "0";

        if (p.colour == colour.BOTH)
            return column8[p.y] + row8[p.x];
        else if (p.colour == colour.BLACK)
        {
            string row = p.x % 2 == 0 ? row16[p.x * 2] : row16[p.x * 2 + 1];
            string column = p.y % 2 == 0 ? column16[p.y * 2 + 1] : column16[p.y * 2];
            //return row + column;
            return column + row;

        }
        else
        {
            string row = p.x % 2 == 0 ? row16[p.x * 2 + 1] : row16[p.x * 2];
            string column = p.y % 2 == 0 ? column16[p.y * 2] : column16[p.y * 2 + 1];
            //return row + column;
            return column + row;

        }

    }
    private void checkRescueAbilitycolour(Character coastGuard)
    {
        var yourPlayers = coastGuard.getOwner() == Owner.USER ? userPlayers : computerPlayers;
        //var yourPlayers = coastGuard.getOwner() == Owner.USER ? computerPlayers : userPlayers;


        foreach (var p in yourPlayers)
        {
            if (p.isAlive() == State.CAPTURE)
            {
                rescuecolour.Add(p.getPosition().colour);
            }
        }

        foreach (var colour in rescuecolour)
        {
            var available_position = availableOfLastRow(coastGuard, colour);

            if (available_position.Count > 0)
            {
                rescuecolour.Add(colour);
                available_promote_point.AddRange(available_position);
            }
        }

    }
    private void updateArrayPosition()
    {
        userPlayersPosition.Clear();
        computerPlayersPosition.Clear();
        projectitlePosition.Clear();
        foreach (var p in userPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                userPlayersPosition.Add(p.getPosition());
            }
        }

        foreach (var p in computerPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                computerPlayersPosition.Add(p.getPosition());
            }
        }

        foreach (var p in userChips)
        {
            if (p.isAlive() == State.ALIVE && Character.IsValidPoint(p.getPosition()))
            {
                projectitlePosition.Add(p.getPosition());
            }
        }

        foreach (var p in computerChips)
        {
            if (p.isAlive() == State.ALIVE && Character.IsValidPoint(p.getPosition()))
            {
                projectitlePosition.Add(p.getPosition());
            }
        }

    }

    private List<Character> attack_presidents =  new List<Character>();
    private int couter_to_clear_attack_list;
    private moveDecission tryToCaptureUser(List<Character> canMove)
    {
        moveDecission result = new moveDecission();
        //List<Character> canMove = new List<Character>();
        Char_Point shouldMoveTo = new Char_Point(-1, -1, colour.BOTH);

        for (int i = 0; i < computerPlayers.Count; i++)
        {
            Character p = computerPlayers[i];
            if (p.isAlive() != State.ALIVE)
                continue;

            List<Char_Point> availableMove = p.availableMovePoints(computerPlayersPosition, userPlayersPosition, projectitlePosition);
            bool king_in_danger = isPresidentInCheck(0);
            bool killed_president = isPresidentInCheck(1);

            if (killed_president)
            {
                shouldMoveTo = getPlayerByName(CharacterDef.PRESIDENT, userPlayers).getPosition();
            }

            if (availableMove.Count > 0)
            {
                canMove.Add(p);
                foreach (Char_Point point in availableMove)
                {
                    if (Character.IsOpponentPlayer(point, computerPlayersPosition, userPlayersPosition, projectitlePosition))
                    {
                        result.moveTo = point;
                        result.player = p;
                        result.available_move = availableMove;

                        if (killed_president == false || (shouldMoveTo.index() == point.index()))
                        {
                            var move_result = updatePositionForPlayer(point, 0, i);
                            if (move_result.isValid)
                            {
                                unDoAction(1);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Validate the move first
        if (result.player != null)
        {
            var move_result = updatePositionForPlayer(result.moveTo, 0, getindexByCharacter(result.player).index);
            if (move_result.isValid)
            {
                unDoAction(1);
            }
            else
            {
                // Should not move this way => might kill the president
                result.player = null;
            }
        }

        return result;

    }

    private moveDecission tryToCaptureBot(List<Character> canMove)
    {
        moveDecission result = new moveDecission();
        //List<Character> canMove = new List<Character>();
        Char_Point shouldMoveTo = new Char_Point(-1, -1, colour.BOTH);

        for (int i = 0; i < userPlayers.Count; i++)
        {
            Character p = userPlayers[i];
            if (p.isAlive() != State.ALIVE)
                continue;

            List<Char_Point> availableMove = p.availableMovePoints(userPlayersPosition, computerPlayersPosition, projectitlePosition);
            bool king_in_danger = isPresidentInCheck(1);
            bool killed_president = isPresidentInCheck(0);

            if (killed_president)
            {
                shouldMoveTo = getPlayerByName(CharacterDef.PRESIDENT, computerPlayers).getPosition();
            }

            if (availableMove.Count > 0)
            {
                canMove.Add(p);
                foreach (Char_Point point in availableMove)
                {
                    if (Character.IsOpponentPlayer(point, userPlayersPosition, computerPlayersPosition, projectitlePosition))
                    {
                        result.moveTo = point;
                        result.player = p;
                        result.available_move = availableMove;

                        if (killed_president == false || (shouldMoveTo.index() == point.index()))
                        {
                            var move_result = updatePositionForPlayer(point, 1, i);
                            if (move_result.isValid)
                            {
                                unDoAction(1);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Validate the move first
        if (result.player != null)
        {
            var move_result = updatePositionForPlayer(result.moveTo, 1, getindexByCharacter(result.player).index);
            if (move_result.isValid)
            {
                unDoAction(1);
            }
            else
            {
                // Should not move this way => might kill the president
                result.player = null;
            }
        }

        return result;

    }

    /* public void SetTurn(TurnState turnState)
     {
         _turnState = turnState;
     }

     public TurnState GetTurn()
     {
         return _turnState;
     }*/


    private moveDecission getRandomMove(List<Character> canMove)
    {
        moveDecission result = new moveDecission() ;

        while (result.player == null)
        {
            // Take 1 turn from 5 to throw projectile
            if (UnityEngine.Random.Range(1, 6) == 5)
            {
                result = throwProjectileForComputer();
                if (result.player != null)
                    return result;
            }

            // Pick a random player from the available players
            int random_index = UnityEngine.Random.Range(0, canMove.Count);
            result.player = canMove[random_index];
            result.available_move = result.player.availableMovePoints(computerPlayersPosition, userPlayersPosition, projectitlePosition);
            int count = 0;

            while (count < 4)
            {
                random_index = UnityEngine.Random.Range(0, result.available_move.Count);
                result.moveTo = result.available_move[random_index];

                if (result.available_move[random_index].y >= result.player.getPosition().y)
                {
                    count++;
                    continue;
                }
                else
                {
                    break;
                }
            }

            var move_result = updatePositionForPlayer(result.moveTo, 0, getindexByCharacter(result.player).index);
            if (move_result.isValid)
            {
                unDoAction(1);
                break;
            }
            else
            {
                result.player = null;
            }
        }

        return result;

    }

    private moveDecission getRandomMoveForPlayerTeamBot(List<Character> canMove)
    {
        moveDecission result = new moveDecission();

        while (result.player == null)
        {
            // Take 1 turn from 5 to throw projectile
            if (UnityEngine.Random.Range(1, 6) == 5)
            {
                result = throwProjectileForComputerPlayerTeamBot();
                if (result.player != null)
                    return result;
            }

            // Pick a random player from the available players
            int random_index = UnityEngine.Random.Range(0, canMove.Count);
            result.player = canMove[random_index];
            result.available_move = result.player.availableMovePoints(userPlayersPosition, computerPlayersPosition, projectitlePosition);
            int count = 0;

            while (count < 4)
            {
                random_index = UnityEngine.Random.Range(0, result.available_move.Count);
                result.moveTo = result.available_move[random_index];

                if (result.available_move[random_index].y >= result.player.getPosition().y)
                {
                    count++;
                    continue;
                }
                else
                {
                    break;
                }
            }

            var move_result = updatePositionForPlayer(result.moveTo, 1, getindexByCharacter(result.player).index);
            if (move_result.isValid)
            {
                unDoAction(1);
                break;
            }
            else
            {
                result.player = null;
            }
        }

        return result;

    }

    private moveDecission tryToAttackUserPresident(List<Character> canMove)
    {
        List<moveDecission> result = new List<moveDecission>();
        Character userPresident = getPlayerByName(CharacterDef.PRESIDENT, userPlayers);
        Char_Point posAttack = userPresident.getPosition();
        List<Char_Point> presidentCankill = userPresident.pointCanKillOpponent(userPlayersPosition, computerPlayersPosition, projectitlePosition);
        Char_Point keep_original_pos;
        List<Char_Point> canKill = new List<Char_Point>();
        List<Char_Point> availableMove = new List<Char_Point>();

        foreach (Character p in canMove)
        {
            if (p.isAlive() != State.ALIVE)
                continue;

            keep_original_pos = p.getPosition();
            availableMove = p.availableMovePoints(computerPlayersPosition, userPlayersPosition, projectitlePosition);

            foreach (Char_Point check_point in availableMove)
            {
                p.setPosition(check_point);
                canKill = p.pointCanKillOpponent(computerPlayersPosition, userPlayersPosition, projectitlePosition);

                foreach (Char_Point kill_point in canKill)
                {
                    if (kill_point.index() == posAttack.index())
                    {
                        moveDecission tmp = new moveDecission();
                        tmp.player = p;
                        tmp.available_move = availableMove;
                        tmp.moveTo = check_point;
                        result.Add(tmp);
                    }
                }
            }

            p.setPosition(keep_original_pos);
        }

        if (result.Count > 0)
        {
            int random_index = UnityEngine.Random.Range(0, result.Count);
            moveDecission selectedMove = result[random_index];
            var move_result = updatePositionForPlayer(selectedMove.moveTo, 0, getindexByCharacter(selectedMove.player).index);

            if (move_result.isValid)
            {
                unDoAction(1);
            }
            else
            {
                return new moveDecission(); // Return an empty move decision if the move is not valid
            }

            foreach (Char_Point check_point in presidentCankill)
            {
                if (check_point.index() == selectedMove.moveTo.index())
                {
                    selectedMove.player = null;
                    break;
                }
            }

            return selectedMove;
        }
        else
        {
            return new moveDecission(); // Return an empty move decision if there are no valid moves
        }

    }

    private moveDecission tryToAttackBotPresident(List<Character> canMove)
    {
        List<moveDecission> result = new List<moveDecission>();
        Character userPresident = getPlayerByName(CharacterDef.PRESIDENT, userPlayers);
        Char_Point posAttack = userPresident.getPosition();
        List<Char_Point> presidentCankill = userPresident.pointCanKillOpponent(userPlayersPosition, computerPlayersPosition, projectitlePosition);
        Char_Point keep_original_pos;
        List<Char_Point> canKill = new List<Char_Point>();
        List<Char_Point> availableMove = new List<Char_Point>();

        foreach (Character p in canMove)
        {
            if (p.isAlive() != State.ALIVE)
                continue;

            keep_original_pos = p.getPosition();
            availableMove = p.availableMovePoints(computerPlayersPosition, userPlayersPosition, projectitlePosition);

            foreach (Char_Point check_point in availableMove)
            {
                p.setPosition(check_point);
                canKill = p.pointCanKillOpponent(computerPlayersPosition, userPlayersPosition, projectitlePosition);

                foreach (Char_Point kill_point in canKill)
                {
                    if (kill_point.index() == posAttack.index())
                    {
                        moveDecission tmp = new moveDecission();
                        tmp.player = p;
                        tmp.available_move = availableMove;
                        tmp.moveTo = check_point;
                        result.Add(tmp);
                    }
                }
            }

            p.setPosition(keep_original_pos);
        }

        if (result.Count > 0)
        {
            int random_index = UnityEngine.Random.Range(0, result.Count);
            moveDecission selectedMove = result[random_index];
            var move_result = updatePositionForPlayer(selectedMove.moveTo, 0, getindexByCharacter(selectedMove.player).index);

            if (move_result.isValid)
            {
                unDoAction(1);
            }
            else
            {
                return new moveDecission(); // Return an empty move decision if the move is not valid
            }

            foreach (Char_Point check_point in presidentCankill)
            {
                if (check_point.index() == selectedMove.moveTo.index())
                {
                    selectedMove.player = null;
                    break;
                }
            }

            return selectedMove;
        }
        else
        {
            return new moveDecission(); // Return an empty move decision if there are no valid moves
        }

    }

    private moveDecission rescuePresidentFromCheck()
    {
        moveDecission result = new moveDecission();

        // Rescue your president by any means necessary
        for (int i = 0; i < computerPlayers.Count; i++)
        {
            Character player = computerPlayers[i];
            if (player.isAlive() == State.ALIVE)
            {
                List<Char_Point> availableMove = player.availableMovePoints(computerPlayersPosition, userPlayersPosition, projectitlePosition);
                foreach (Char_Point point in availableMove)
                {
                    var move_result = updatePositionForPlayer(point, 0, i);
                    if (move_result.isValid)
                    {
                        unDoAction(1);
                        result.moveTo = point;
                        result.player = player;
                        result.available_move = availableMove;
                        return result;
                    }
                }
            }
        }

        return throwProjectileForComputer();

    }

    private moveDecission rescuePresidentFromCheckPlayerTeamBot()
    {
        moveDecission result = new moveDecission();

        // Rescue your president by any means necessary
        for (int i = 0; i < userPlayers.Count; i++)
        {
            Character player = userPlayers[i];
            if (player.isAlive() == State.ALIVE)
            {
                List<Char_Point> availableMove = player.availableMovePoints(userPlayersPosition, computerPlayersPosition, projectitlePosition);
                foreach (Char_Point point in availableMove)
                {
                    var move_result = updatePositionForPlayer(point, 1, i);
                    if (move_result.isValid)
                    {
                        unDoAction(1);
                        result.moveTo = point;
                        result.player = player;
                        result.available_move = availableMove;
                        return result;
                    }
                }
            }
        }

        return throwProjectileForComputerPlayerTeamBot();

    }

    public void cleanupForNewScene()
    {
        userPlayersPosition.Clear();
        computerPlayersPosition.Clear();
        projectitlePosition.Clear();
        encode_games.Clear();
        couldBeKilledByComp.Clear();
        couldBeKilledByUser.Clear();
        castle_move.Clear();
        isPosibleCastle = false;
        rescuecolour.Clear();
        available_promote_point.Clear();
        isWaitingForPromote = false;

        while (changeTracking.Count > 0)
            changeTracking.Pop();

        foreach (var player in userPlayers)
        {
            player.Dispose();
        }
        userPlayers.Clear();

        foreach (var player in computerPlayers)
        {
            player.Dispose();
        }
        computerPlayers.Clear();

        foreach (var chip in userChips)
        {
            chip.Dispose();
        }
        userChips.Clear();

        foreach (var chip in computerChips)
        {
            chip.Dispose();
        }
        computerChips.Clear();

    }
    public float getPoint(int whichOne) 
    { 
        //return whichOne == 1 ? user_point : comp_point; 
      
        if(whichOne == 1)
        {
            return user_point;
        }
        else if (whichOne == 0)
        {
            return comp_point;
        }
        else if (whichOne == 2)
        {
            return comp2_point;
        }
        else if (whichOne == 3)
        {
            Debug.Log("MA::Get Point Comp3::" + comp3_point);

            return comp3_point;
        }

        return 0;//this should not happen!!!
    }
    public Button findCorrectButton(List<Button> allBoardRects, int whichOne, int index)
    {
        int indexForButton = 0;
        if (index < 0 || index >= userPlayers.Count)
        {
            Debug.Log("Invalid index");
        }
        if (whichOne > 0) // User
            indexForButton = userPlayers[index].getPosition().index();
        else
            indexForButton = computerPlayers[index].getPosition().index();

        return allBoardRects[indexForButton];
    }
    public string getMovingStep()
    {
        string result = "";
        for (int i = 0; i < encode_games.Count; i++)
        {
            if (i % 2 == 0)
            {
                result += "\n";
                result += (i / 2) + ". ";
            }

            result += encode_games[i];
            result += "    ";
        }

        //MA
        try
        {
            result = encode_games.LastOrDefault().ToString();//i want just last step to be returned

        }
        catch(Exception e)
        {

        }

        return result;

    }

    public string getMovingStepAll()
    {
        string result = "";
        for (int i = 0; i < encode_games.Count; i++)
        {
            if (i % 2 == 0)
            {
                result += "\n";
                result += (i / 2) + ". ";
            }

            result += encode_games[i];
            result += "    ";
        }


        return result;

    }

    public List<Character> getUserPlayers() { 
        return userPlayers; }
    public List<Character> getComputerPlayers() { return computerPlayers; }
    public List<Character> getUserChips() { return userChips; }
    public List<Character> getComputerChips() { return computerChips; }
    public List<Char_Point> selectWhereToGo(int index, int whichone)
    {
            List<Char_Point> empty = new List<Char_Point>();
            isPosibleCastle = false;
            Character player;
            HashSet<int> couldBeKilled = whichone > 0 ? couldBeKilledByComp : couldBeKilledByUser;
            List<Char_Point> yourPlayerPosition = whichone>0 ? userPlayersPosition : computerPlayersPosition;
            List<Char_Point> opponentPlayerPosition = whichone>0 ? computerPlayersPosition : userPlayersPosition;

            // Click on Projectile
            if (index >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
            {
                player = index >= (int)GameConstants.PlayerTags.COMP_CHIP_TAG ? computerChips[index - (int)GameConstants.PlayerTags.COMP_CHIP_TAG] : userChips[index - (int)GameConstants.PlayerTags.USER_CHIP_TAG];
                if (player.isAlive() == State.ALIVE)
                    return player.availableMovePoints(yourPlayerPosition, opponentPlayerPosition, projectitlePosition);
                else
                    return empty;
            }

            // Click on Player
            player = whichone> 0 ? userPlayers[index] : computerPlayers[index];

            if (player.isAlive() == State.ALIVE)
            {
                List<Char_Point> result = player.availableMovePoints(yourPlayerPosition, opponentPlayerPosition, projectitlePosition);
                if (player.getName() == CharacterDef.PRESIDENT)
                {
                    for (int i = result.Count - 1; i >= 0; i--)
                    {
                        if (couldBeKilled.Contains(result[i].index()))
                        {
                            result.RemoveAt(i);
                        }
                    }

                    if (player.getIsFirstMove())
                    {
                        List<Char_Point> castlePoints = castleCheck(whichone);
                        if (castlePoints.Count > 0)
                        {
                            isPosibleCastle = true;
                            result.AddRange(castlePoints);
                        }
                    }
                }
                return result;
            }

            return empty;
        


    }
    public List<Char_Point> selectPositionToPromote(int index, int whichone)
    {
        List<Char_Point> result = new List<Char_Point>();

        if (index < userPlayers.Count)
        {
            var player = whichone>0 ? userPlayers[index] : computerPlayers[index];

            if (player.isAlive() == State.CAPTURE && rescuecolour.Count > 0)
            {
                foreach (var p in available_promote_point)
                {
                    if (player.getPosition().colour == p.colour)
                    {
                        result.Add(p);
                    }
                }
            }
        }

        return result;

    }
    public updatePosStatus updatePositionForPlayer(Char_Point movePos, int whichOne, int index)
    {
        updatePosStatus result = new updatePosStatus();
        var opponentPlayers = whichOne>0 ? computerPlayers : userPlayers;
        var player = whichOne>0 ? userPlayers[index] : computerPlayers[index];

      

        // changeTracking;
        List<moveStatus> stat_change = new List<moveStatus>();
        
        stat_change.Add(new moveStatus(player, player.getPosition(), player.getIsFirstMove(), player.isAlive()));

        for (int i = 0; i < opponentPlayers.Count; i++)
        {
            
                if (checkCaptureKillPlayer(player, opponentPlayers[i], movePos))
                {
                    stat_change.Add(new moveStatus(opponentPlayers[i], opponentPlayers[i].getPosition(), opponentPlayers[i].getIsFirstMove(), State.ALIVE));
                    result.removePlayers.Add(i);
                }

            
        }

        List<int> removalchips = checkIfMoveCanRemoveChips(player, movePos, stat_change);
        foreach (var p in removalchips)
        {
            result.removePlayers.Add(p);
        }

        if (isPosibleCastle && player.getName() == CharacterDef.PRESIDENT)
        {
            updateCastleMoveIfany(movePos, player, whichOne, stat_change);
        }

        /*if (player.isAlive() == State.ALIVE && player.getName() == CharacterDef.COASTGUARD && ((CoastGuard)player).goToTheEnd(movePos))
        {
            player.setPosition(movePos);
            updateStatusAfterMove();
            checkRescueAbilitycolour(player);
            if (rescuecolour.Count == 0)
                isWaitingForPromote = true;
            else
                isWaitingForPromote = false;
        }*/

        // for rescue the when Coast-Guard go to the end.
        if (player.isAlive() == State.CAPTURE)
        {
            rescuecolour.Clear();
            available_promote_point.Clear();
            player.SetAlive(State.ALIVE);

            encode_games[encode_games.Count - 1] += player.getEncodeName() + "" + encodeCharPoint(player.getPosition()) + "-" + encodeCharPoint(movePos);

        }
        else
        {
           

            if (result.removePlayers.Count > 0)
                encode_games.Add(player.getEncodeName() + "" + encodeCharPoint(player.getPosition()) + "-x" + encodeCharPoint(movePos));
            else
                encode_games.Add(player.getEncodeName() + "" + encodeCharPoint(player.getPosition()) + "-" + encodeCharPoint(movePos));
        }

        player.setPosition(movePos);
        player.setIsFirstMove(false);
        changeTracking.Push(stat_change);
        updateStatusAfterMove();
        isPosibleCastle = false;
        if (isPresidentInCheck(whichOne))
        {
            result.isValid = false;
            unDoAction(1);
        }

        return result;

    }
    public updatePosStatus updatePositionForChip(Char_Point movePos, int whichOne, int index)
    {
        updatePosStatus result = new updatePosStatus();

        Character chip;
        List<moveStatus> stat_change = new List<moveStatus>();
        if (index >= (int)GameConstants.PlayerTags.COMP_CHIP_TAG)
        {
            chip = computerChips[index - (int)GameConstants.PlayerTags.COMP_CHIP_TAG];
        }
        else if (index >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
        {
            chip = userChips[index - (int)GameConstants.PlayerTags.USER_CHIP_TAG];
        }
        else
        {
            Debug.Log("Invalid index " + index);
            return result;
        }
        stat_change.Add(new moveStatus(chip, chip.getPosition(), chip.getIsFirstMove(), chip.isAlive()));

        for (int i = 0; i < userPlayers.Count; i++)
        {
            if (checkCaptureKillPlayer(chip, userPlayers[i], movePos))
            {
                stat_change.Add(new moveStatus(userPlayers[i], userPlayers[i].getPosition(), userPlayers[i].getIsFirstMove(), State.ALIVE));
                result.removePlayers.Add(i);
            }
        }

        for (int i = 0; i < computerPlayers.Count; i++)
        {
            if (checkCaptureKillPlayer(chip, computerPlayers[i], movePos))
            {
                stat_change.Add(new moveStatus(computerPlayers[i], computerPlayers[i].getPosition(), computerPlayers[i].getIsFirstMove(), State.ALIVE));
                result.removePlayers.Add(i);
            }
        }

        for (int i = 0; i < userChips.Count; i++)
        {
            if (Character.IsValidPoint(userChips[i].getPosition()) && checkCaptureKillPlayer(chip, userChips[i], movePos))
            {
                stat_change.Add(new moveStatus(userChips[i], userChips[i].getPosition(), userChips[i].getIsFirstMove(), State.ALIVE));
                result.removePlayers.Add(i + (int)GameConstants.PlayerTags.USER_CHIP_TAG);
            }
        }

        for (int i = 0; i < computerChips.Count; i++)
        {
            if (Character.IsValidPoint(computerChips[i].getPosition()) && checkCaptureKillPlayer(chip, computerChips[i], movePos))
            {
                stat_change.Add(new moveStatus(computerChips[i], computerChips[i].getPosition(), computerChips[i].getIsFirstMove(), State.ALIVE));
                result.removePlayers.Add(i + (int)GameConstants.PlayerTags.COMP_CHIP_TAG);
            }
        }

        
        
        if (result.removePlayers.Count > 0)
        {
            if (chip.getName() == CharacterDef.ATOMIC || chip.getName() == CharacterDef.HYD_NUKE)
            {
                chip.SetAlive(State.DIED);


                encode_games.Add(chip.getEncodeName() + "(" + 0 + ")k" + encodeCharPoint(movePos));
            }
            else
            {
                encode_games.Add(chip.getEncodeName() + "(" + 0 + ")x" + encodeCharPoint(movePos));
            }
        }
        else
        {
            encode_games.Add(chip.getEncodeName() + "(" + 0 + ")" + encodeCharPoint(movePos));
            chip.setPosition(movePos);
        }
        changeTracking.Push(stat_change);
        updateStatusAfterMove();
        return result;

    }
    public updatePosStatus updatePosition(Char_Point movePos, int whichOne, int index)
    {
        castle_move.Clear();
        if (index >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
            return updatePositionForChip(movePos, whichOne, index);
        else
            return updatePositionForPlayer(movePos, whichOne, index);

    }
    public HashSet<colour> colourToRescueCapturePlayer() { return rescuecolour; }
    public void updateStatusAfterMove()
    {
        updatePoint();
        updateArrayPosition();
        updateCouldBeKilled();
    }
    public List<List<moveStatus>> unDoAction(int steps = 2, bool isTemp = true)
    {
        List<List<moveStatus>> unDoList = new List<List<moveStatus>>();
        for (int i = 0; i < steps; i++)
        {
            if (changeTracking.Count > 0)
            {
                List<moveStatus> oneMove = changeTracking.Peek();
                unDoList.Add(oneMove);
                changeTracking.Pop();
                foreach (var undo in oneMove)
                {
                    undo.reverse_status();
                }
            }
            if (encode_games.Count > 0)
            {
                encode_games.RemoveAt(encode_games.Count - 1);
            }

            if (!isTemp)
            {
                if (turnStateSeqnce.Count > 0)
                {
                    turnStateSeqnce.Pop();
                }
                //SetTurn(turnStateSeqnce.Peek());
            }
        }

        updateStatusAfterMove();
        

        return unDoList;

    }
    public indexPlayer getindexByCharacter(Character player)
    {
        indexPlayer ret = new indexPlayer { index = -1, who = -1 };
        for (int i = 0; i < userPlayers.Count; i++)
        {
            if (userPlayers[i] == player)
            {
                ret.index = i;
                ret.who = 1;
                return ret;
            }
        }
        for (int i = 0; i < computerPlayers.Count; i++)
        {
            if (computerPlayers[i] == player)
            {
                ret.index = i;
                ret.who = 0;
                return ret;
            }
        }

        for (int i = 0; i < userChips.Count; i++)
        {
            if (userChips[i] == player)
            {
                ret.index = i + (int)GameConstants.PlayerTags.USER_CHIP_TAG;
                ret.who = 1;
                return ret;
            }
        }

        for (int i = 0; i < computerChips.Count; i++)
        {
            if (computerChips[i] == player)
            {
                ret.index = i + (int)GameConstants.PlayerTags.COMP_CHIP_TAG;
                ret.who = 0;
                return ret;
            }
        }

        return ret;

    }
    public List<Character> getCastleMove() { return castle_move; }
    public bool needPermanentRemove(int tag)
    {
        Character chip;
        if (tag >= (int)GameConstants.PlayerTags.COMP_CHIP_TAG)
        {
            chip = computerChips[tag - (int)GameConstants.PlayerTags.COMP_CHIP_TAG];

        }
        else if (tag >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
        {
            chip = userChips[tag - (int)GameConstants.PlayerTags.USER_CHIP_TAG];
        }
        else
        {
            return false;
        }

        if (chip.isAlive() == State.DIED)
        {
            
            return true;
        }
        else
        {
            return false;
        }

    }
    public void updateCouldBeKilled()
    {
        couldBeKilledByComp.Clear();
        couldBeKilledByUser.Clear();
        foreach (var p in userPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                var canKill = p.pointCanKillOpponent(userPlayersPosition, computerPlayersPosition, projectitlePosition);
                foreach (var pos in canKill)
                {
                    if (pos.index() >= 0 && pos.index() <= 77)
                        couldBeKilledByUser.Add(pos.index());
                }
            }
        }

        foreach (var p in computerPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                var canKill = p.pointCanKillOpponent(computerPlayersPosition, userPlayersPosition, projectitlePosition);
                foreach (var pos in canKill)
                {
                    if (pos.index() >= 0 && pos.index() <= 77)
                        couldBeKilledByComp.Add(pos.index());
                }
            }
        }

    }

    public void updatePoint()
    {
        user_point = 0;
        comp_point = 0;
        comp2_point = 0;
        comp3_point = 0;
        foreach (var p in userPlayers)
        {
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && !p.isOutByComp2())
            {
                comp_point += p.getValue();
                
                
            }
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && p.isOutByComp2())
            {
                comp2_point += p.getValue();
            }
        }

        

        foreach (var p in computerPlayers)
        {
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && p.isOutByComp3())
            {
                Debug.Log("IS out by Comp3::");
                comp3_point += p.getValue();
            }
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && !p.isOutByComp3())
            {
                Debug.Log("IS out by User::");
                user_point += p.getValue();
            }
            
        }

      
    }

    public void setSwitchPresidentPos(bool x) 
    { isSwitchPresidentPostion = x;
    }

    public void setIsGameTypeChance(bool x)
    {
        isGameTypeChance = x;
    }

    public void initBoardData(bool isBlack)
    {
        int val = PlayerPrefs.GetInt("switchChar", 0);
        setSwitchPresidentPos(val == 1);

        // User Players
        userPlayers.Add(createCharacter(new Char_Point(0, 0, colour.BLACK), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(0, 0, colour.WHITE), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(1, 0, colour.BLACK), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(1, 0, colour.WHITE), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(2, 0, colour.BLACK), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(2, 0, colour.WHITE), CharacterDef.AIRFORCE, Owner.USER));

       
            if (isSwitchPresidentPostion)
            {
                userPlayers.Add(createCharacter(new Char_Point(4, 0, colour.BOTH), CharacterDef.ARMY, Owner.USER));
                userPlayers.Add(createCharacter(new Char_Point(3, 0, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
            }
            else
            {
                userPlayers.Add(createCharacter(new Char_Point(3, 0, colour.BOTH), CharacterDef.ARMY, Owner.USER));
                userPlayers.Add(createCharacter(new Char_Point(4, 0, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
            }

           
       

        

        userPlayers.Add(createCharacter(new Char_Point(5, 0, colour.BLACK), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(5, 0, colour.WHITE), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(6, 0, colour.BLACK), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(6, 0, colour.WHITE), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(7, 0, colour.BLACK), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(7, 0, colour.WHITE), CharacterDef.MARINE, Owner.USER));




        for (int i = 0; i < 8; i++)
        {
            userPlayers.Add(createCharacter(new Char_Point(i, 1, colour.BLACK), CharacterDef.COASTGUARD, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(i, 1, colour.WHITE), CharacterDef.COASTGUARD, Owner.USER));
        }

        // Computer Players
        computerPlayers.Add(createCharacter(new Char_Point(0, 7, colour.BLACK), CharacterDef.MARINE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(0, 7, colour.WHITE), CharacterDef.MARINE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(1, 7, colour.BLACK), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(1, 7, colour.WHITE), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(2, 7, colour.BLACK), CharacterDef.AIRFORCE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(2, 7, colour.WHITE), CharacterDef.AIRFORCE, Owner.COMPUTER));

       


        computerPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.ARMY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.PRESIDENT, Owner.COMPUTER));

        computerPlayers.Add(createCharacter(new Char_Point(5, 7, colour.BLACK), CharacterDef.AIRFORCE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(5, 7, colour.WHITE), CharacterDef.AIRFORCE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(6, 7, colour.BLACK), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(6, 7, colour.WHITE), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(7, 7, colour.BLACK), CharacterDef.MARINE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(7, 7, colour.WHITE), CharacterDef.MARINE, Owner.COMPUTER));


        for (int i = 0; i < 8; i++)
        {
            computerPlayers.Add(createCharacter(new Char_Point(i, 6, colour.BLACK), CharacterDef.COASTGUARD, Owner.COMPUTER));
            computerPlayers.Add(createCharacter(new Char_Point(i, 6, colour.WHITE), CharacterDef.COASTGUARD, Owner.COMPUTER));
        }

        this.isBlack = isBlack;
        updateArrayPosition();
        updateStatusAfterMove();

    }

    int[] randPosGOC = {0,1,2,3,4,5,6,7 };
    public void Reshuffle()
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < randPosGOC.Length; t++)
        {
            int tmp = randPosGOC[t];
            int r = UnityEngine.Random.Range(t, randPosGOC.Length);
            randPosGOC[t] = randPosGOC[r];
            randPosGOC[r] = tmp;
        }
    }


    public void initBoardDataGOC(bool isBlack)
    {
        int val = PlayerPrefs.GetInt("switchChar", 0);
        setSwitchPresidentPos(val == 1);

        Reshuffle();

        // User Players
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[0], 0, colour.BLACK), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[2], 0, colour.WHITE), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[1], 0, colour.BLACK), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[0], 0, colour.WHITE), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[2], 0, colour.BLACK), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[1], 0, colour.WHITE), CharacterDef.AIRFORCE, Owner.USER));


        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[3], 0, colour.BOTH), CharacterDef.ARMY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[4], 0, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
        

        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[5], 0, colour.BLACK), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[7], 0, colour.WHITE), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[6], 0, colour.BLACK), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[5], 0, colour.WHITE), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[6], 0, colour.WHITE), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(randPosGOC[7], 0, colour.BLACK), CharacterDef.MARINE, Owner.USER));




        for (int i = 0; i < 8; i++)
        {
            userPlayers.Add(createCharacter(new Char_Point(i, 1, colour.BLACK), CharacterDef.COASTGUARD, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(i, 1, colour.WHITE), CharacterDef.COASTGUARD, Owner.USER));
        }

        Reshuffle();


        // Computer Players
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[0], 7, colour.BLACK), CharacterDef.MARINE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[2], 7, colour.WHITE), CharacterDef.MARINE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[1], 7, colour.BLACK), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[0], 7, colour.WHITE), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[2], 7, colour.BLACK), CharacterDef.AIRFORCE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[1], 7, colour.WHITE), CharacterDef.AIRFORCE, Owner.COMPUTER));




        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[3], 7, colour.BOTH), CharacterDef.ARMY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[4], 7, colour.BOTH), CharacterDef.PRESIDENT, Owner.COMPUTER));

        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[5], 7, colour.BLACK), CharacterDef.AIRFORCE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[7], 7, colour.WHITE), CharacterDef.AIRFORCE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[6], 7, colour.BLACK), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[5], 7, colour.WHITE), CharacterDef.NAVY, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[6], 7, colour.WHITE), CharacterDef.MARINE, Owner.COMPUTER));
        computerPlayers.Add(createCharacter(new Char_Point(randPosGOC[7], 7, colour.BLACK), CharacterDef.MARINE, Owner.COMPUTER));


        for (int i = 0; i < 8; i++)
        {
            computerPlayers.Add(createCharacter(new Char_Point(i, 6, colour.BLACK), CharacterDef.COASTGUARD, Owner.COMPUTER));
            computerPlayers.Add(createCharacter(new Char_Point(i, 6, colour.WHITE), CharacterDef.COASTGUARD, Owner.COMPUTER));
        }

        this.isBlack = isBlack;
        updateArrayPosition();
        updateStatusAfterMove();

    }

    public void initChips()
    {
        // User Chips
        for (int i = 0; i < 8; i++)
        {
            if (i % 2 == 0)
            {
                userChips.Add(createCharacter(new Char_Point(i, -1, colour.BLACK), CharacterDef.POISON_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(i, -1, colour.WHITE), CharacterDef.ATOMIC, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(i, -2, colour.BOTH), CharacterDef.HYD_NUKE, Owner.USER));
            }
            else
            {
                userChips.Add(createCharacter(new Char_Point(i, -1, colour.WHITE), CharacterDef.POISON_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(i, -1, colour.BLACK), CharacterDef.ATOMIC, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(i, -2, colour.BOTH), CharacterDef.BIO_GAS, Owner.USER));
            }
        }

        // User Chips in columns
        for (int i = 0; i < 4; i++)
        {
            if (i % 2 == 0)
            {
                userChips.Add(createCharacter(new Char_Point(-1, i, colour.BLACK), CharacterDef.POISON_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(-1, i, colour.WHITE), CharacterDef.ATOMIC, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(8, i, colour.WHITE), CharacterDef.POISON_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(8, i, colour.BLACK), CharacterDef.ATOMIC, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(-2, i, colour.BOTH), CharacterDef.BIO_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(9, i, colour.BOTH), CharacterDef.HYD_NUKE, Owner.USER));
            }
            else
            {
                userChips.Add(createCharacter(new Char_Point(-1, i, colour.WHITE), CharacterDef.POISON_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(-1, i, colour.BLACK), CharacterDef.ATOMIC, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(8, i, colour.BLACK), CharacterDef.POISON_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(8, i, colour.WHITE), CharacterDef.ATOMIC, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(9, i, colour.BOTH), CharacterDef.BIO_GAS, Owner.USER));
                userChips.Add(createCharacter(new Char_Point(-2, i, colour.BOTH), CharacterDef.HYD_NUKE, Owner.USER));
            }
        }

        // Computer Chips
        for (int i = 0; i < 8; i++)
        {
            if (i % 2 == 0)
            {
                computerChips.Add(createCharacter(new Char_Point(i, 8, colour.WHITE), CharacterDef.POISON_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(i, 8, colour.BLACK), CharacterDef.ATOMIC, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(i, 9, colour.BOTH), CharacterDef.BIO_GAS, Owner.COMPUTER));
            }
            else
            {
                computerChips.Add(createCharacter(new Char_Point(i, 8, colour.BLACK), CharacterDef.POISON_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(i, 8, colour.WHITE), CharacterDef.ATOMIC, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(i, 9, colour.BOTH), CharacterDef.HYD_NUKE, Owner.COMPUTER));
            }
        }

        // Computer Chips in columns
        for (int i = 4; i < 8; i++)
        {
            if (i % 2 == 0)
            {
                computerChips.Add(createCharacter(new Char_Point(-1, i, colour.BLACK), CharacterDef.POISON_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(-1, i, colour.WHITE), CharacterDef.ATOMIC, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(8, i, colour.WHITE), CharacterDef.POISON_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(8, i, colour.BLACK), CharacterDef.ATOMIC, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(-2, i, colour.BOTH), CharacterDef.BIO_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(9, i, colour.BOTH), CharacterDef.HYD_NUKE, Owner.COMPUTER));
            }
            else
            {
                computerChips.Add(createCharacter(new Char_Point(-1, i, colour.WHITE), CharacterDef.POISON_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(-1, i, colour.BLACK), CharacterDef.ATOMIC, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(8, i, colour.BLACK), CharacterDef.POISON_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(8, i, colour.WHITE), CharacterDef.ATOMIC, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(9, i, colour.BOTH), CharacterDef.BIO_GAS, Owner.COMPUTER));
                computerChips.Add(createCharacter(new Char_Point(-2, i, colour.BOTH), CharacterDef.HYD_NUKE, Owner.COMPUTER));
            }
        }

        updatePoint();

    }
    public List<Char_Point> castleCheck(int whichOne)
    {

        List<Char_Point> posible_castle = new List<Char_Point>();
        Character president;
        Character marine_left_1;
        Character marine_left_2;
        Character marine_right_1;
        Character marine_right_2;

        if (whichOne>0) //user
        {
            president = getPlayerByName(CharacterDef.PRESIDENT, userPlayers);
            marine_left_1 = userPlayers[0];
            marine_left_2 = userPlayers[1];
            marine_right_1 = userPlayers[12];
            marine_right_2 = userPlayers[13];
        }
        else
        {
            president = getPlayerByName(CharacterDef.PRESIDENT, computerPlayers);
            marine_left_1 = computerPlayers[0];
            marine_left_2 = computerPlayers[1];
            marine_right_1 = computerPlayers[12];
            marine_right_2 = computerPlayers[13];
        }

        if (!president.getIsFirstMove() || isPresidentInCheck(whichOne))
        {
            return posible_castle;
        }

        if (marine_left_1.isAlive() == State.ALIVE && marine_left_2.isAlive() == State.ALIVE
            && marine_left_1.getIsFirstMove() && marine_left_2.getIsFirstMove())
        {
            bool can_castle = true;
            Char_Point check_point = president.getPosition();
            Char_Point left_point = president.getPosition();

            while (left_point.x > marine_left_1.getPosition().x + 1)
            {
                check_point = left_point;
                if (!Character.CheckIfEmptyLeft(check_point, userPlayersPosition, computerPlayersPosition, projectitlePosition, ref left_point))
                {
                    can_castle = false;
                    break;
                }
            }

            if (can_castle)
            {
                posible_castle.Add(left_point);
            }
        }

        if (marine_right_1.isAlive() == State.ALIVE && marine_right_2.isAlive() == State.ALIVE &&
            marine_right_1.getIsFirstMove() && marine_right_2.getIsFirstMove())
        {
            bool can_castle = true;
            Char_Point check_point = president.getPosition();
            Char_Point right_point = president.getPosition();

            while (right_point.x < marine_right_1.getPosition().x - 1)
            {
                check_point = right_point;
                if (!Character.CheckIfEmptyRight(check_point, userPlayersPosition, computerPlayersPosition, projectitlePosition, ref right_point))
                {
                    can_castle = false;
                    break;
                }
            }

            if (can_castle)
            {
                posible_castle.Add(right_point);
            }
        }

        int isGameOfChance = PlayerPrefs.GetInt("isChance", 0);
        if (isGameOfChance == 1)
        {
            //castle is not applicable in game of chance
            posible_castle.Clear();
        }

        return posible_castle;

    }
    public List<int> checkIfMoveCanRemoveChips(Character player, Char_Point movePos, List<moveStatus> stat_change)
    {
        List<int> result = new List<int>();
        Char_Point posible_chip_point1 = new Char_Point(-1, -1, colour.BOTH);
        Char_Point posible_chip_point2 = new Char_Point(-1, -1, colour.BOTH);

        if (player.getName() != CharacterDef.COASTGUARD && player.getName() != CharacterDef.NAVY)
        {
            return result;
        }

        if (player.getName() == CharacterDef.COASTGUARD)
        {
            if (player.getPosition().x == movePos.x && Math.Abs(player.getPosition().y - movePos.y) == 2)
            {
                posible_chip_point1 = new Char_Point(movePos.x, (player.getPosition().y + movePos.y) / 2, movePos.colour);
            }
            else if (player.getPosition().y == movePos.y && Math.Abs(player.getPosition().x - movePos.x) == 2)
            {
                posible_chip_point1 = new Char_Point((movePos.x + player.getPosition().x) / 2, movePos.y, movePos.colour);
            }
            else if (Math.Abs(player.getPosition().y - movePos.y) == 1 && (Math.Abs(player.getPosition().x - movePos.x) == 1))
            {
                posible_chip_point1 = new Char_Point(player.getPosition().x, movePos.y, movePos.colour);
                posible_chip_point2 = new Char_Point(movePos.x, player.getPosition().y, movePos.colour);
            }
        }
        else
        {
            if (Math.Abs(movePos.y - player.getPosition().y) == 2)
            {
                posible_chip_point1 = new Char_Point(player.getPosition().x, movePos.y, colour.BOTH);
                posible_chip_point2 = new Char_Point(player.getPosition().x, (movePos.y + player.getPosition().y) / 2, colour.BOTH);
            }
            else
            {
                posible_chip_point1 = new Char_Point(movePos.x, player.getPosition().y, colour.BOTH);
                posible_chip_point2 = new Char_Point((movePos.x + player.getPosition().x) / 2, player.getPosition().y, colour.BOTH);
            }
        }

        foreach (var posible_chip_point in new List<Char_Point> { posible_chip_point1, posible_chip_point2 })
        {
            if (Character.IsValidPoint(posible_chip_point))
            {
                for (int i = 0; i < userChips.Count; i++)
                {
                    if (userChips[i].isAlive() == State.ALIVE && (userChips[i].getPosition().x == posible_chip_point.x && userChips[i].getPosition().y == posible_chip_point.y))
                    {
                        bool isApplicable = false;
                        if (player.getName() == CharacterDef.COASTGUARD)
                        {
                            if (userChips[i].getName() == CharacterDef.POISON_GAS ||
                            userChips[i].getName() == CharacterDef.ATOMIC)
                            {

                                isApplicable = true;
                            }
                        }
                        else if(player.getName() == CharacterDef.NAVY)
                        {
                            if (userChips[i].getName() == CharacterDef.BIO_GAS ||
                            userChips[i].getName() == CharacterDef.HYD_NUKE)
                            {

                                isApplicable = true;
                            }
                        }

                        if (isApplicable)
                        {

                            stat_change.Add(new moveStatus
                            (
                                userChips[i],
                                userChips[i].getPosition(),
                                userChips[i].getIsFirstMove(),
                                State.ALIVE
                            ));

                            if (player.getOwner() == userChips[i].getOwner())
                            {
                                // Return to initial position
                                userChips[i].SetAlive(State.ALIVE);
                                userChips[i].setPosition(userChips[i].getInitPos());
                            }
                            else
                            {
                                userChips[i].SetAlive(State.DIED);
                            }

                            result.Add(i + (int)GameConstants.PlayerTags.USER_CHIP_TAG);
                        }
                    }
                }

                for (int i = 0; i < computerChips.Count; i++)
                {
                    if (computerChips[i].isAlive() == State.ALIVE && (computerChips[i].getPosition().x == posible_chip_point.x && computerChips[i].getPosition().y == posible_chip_point.y))
                    {
                        bool isApplicable = false;
                        if (player.getName() == CharacterDef.COASTGUARD)
                        {
                            if (computerChips[i].getName() == CharacterDef.POISON_GAS ||
                            computerChips[i].getName() == CharacterDef.ATOMIC)
                            {

                                isApplicable = true;
                            }
                        }
                        else if (player.getName() == CharacterDef.NAVY)
                        {
                            if (computerChips[i].getName() == CharacterDef.BIO_GAS ||
                            computerChips[i].getName() == CharacterDef.HYD_NUKE)
                            {

                                isApplicable = true;
                            }
                        }

                        if (isApplicable)
                        {

                            stat_change.Add(new moveStatus
                            (
                            computerChips[i],
                            computerChips[i].getPosition(),
                            computerChips[i].getIsFirstMove(),
                            State.ALIVE
                            ));

                            if (player.getOwner() == computerChips[i].getOwner())
                            {
                                // Return to initial position
                                computerChips[i].SetAlive(State.ALIVE);
                                computerChips[i].setPosition(computerChips[i].getInitPos());
                            }
                            else
                            {
                                computerChips[i].SetAlive(State.DIED);
                            }

                            result.Add(i + (int)GameConstants.PlayerTags.COMP_CHIP_TAG);
                        }
                    }
                }
            }
        }

        return result;

    }
    public bool checkCaptureKillPlayer(Character movePlayer, Character landOnPlayer, Char_Point movePos)
    {
        bool result = false;

        if (landOnPlayer.isAlive() == State.ALIVE &&
            landOnPlayer != movePlayer &&
            landOnPlayer.getPosition().index() == movePos.index() &&
            (landOnPlayer.getPosition().colour == colour.BOTH ||  // 1 colour can capture both
                movePos.colour == landOnPlayer.getPosition().colour ||  // can capture same colour
                movePos.colour == colour.BOTH))
        { // Army, President can capture black/white

            
            
            if (movePlayer.isChip() && landOnPlayer.isChip())
            {
                movePlayer.SetAlive(State.DIED);
                landOnPlayer.SetAlive(State.DIED);

                if(turnStateSeqnce.Peek() == TurnState.OPP2_BOT_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == TurnState.FRND_BOT_TURN)
                {
                    Debug.Log("FRND BOT TURN LAND ON CHIP::" + turnStateSeqnce.Peek().ToString());
                    landOnPlayer.setIsOutByComp3(true);
                }

            }
            else if (movePlayer.getName() == CharacterDef.ATOMIC )
            {
                // removed from the game.
                landOnPlayer.SetAlive(State.DIED);

                movePlayer.SetAlive(State.DIED);

                if (turnStateSeqnce.Peek() == TurnState.OPP2_BOT_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == TurnState.FRND_BOT_TURN)
                {
                    Debug.Log("FRND BOT TURN LAND ON CHIP::" + turnStateSeqnce.Peek().ToString());

                    landOnPlayer.setIsOutByComp3(true);
                }

            }
            else if (movePlayer.getName() == CharacterDef.HYD_NUKE)
            {
                movePlayer.SetAlive(State.DIED);
                landOnPlayer.SetAlive(State.DIED);

                if (turnStateSeqnce.Peek() == TurnState.OPP2_BOT_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == TurnState.FRND_BOT_TURN)
                {

                    Debug.Log("FRND BOT TURN LAND ON CHIP::" + turnStateSeqnce.Peek().ToString());

                    landOnPlayer.setIsOutByComp3(true);
                }
            }
            else if(movePlayer.getName() == CharacterDef.POISON_GAS)
            {
                movePlayer.SetAlive(State.DIED);
                landOnPlayer.SetAlive(State.CAPTURE);

                if (turnStateSeqnce.Peek() == TurnState.OPP2_BOT_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == TurnState.FRND_BOT_TURN)
                {
                    landOnPlayer.setIsOutByComp3(true);
                }
            }
            else
            {
                // POW
                landOnPlayer.SetAlive(State.CAPTURE);

                if (turnStateSeqnce.Peek() == TurnState.OPP2_BOT_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == TurnState.FRND_BOT_TURN)
                {

                    Debug.Log("FRND BOT TURN LAND ON CHAR::"+ turnStateSeqnce.Peek().ToString());

                    landOnPlayer.setIsOutByComp3(true);
                }
            }
            result = true;
        }

        return result;

    }
    public void updateCastleMoveIfany(Char_Point movePos, Character player, int whichOne, List<moveStatus> stat_change)
    {
        Char_Point castle_1 = new Char_Point(1, player.getPosition().y, colour.BOTH);
        Char_Point castle_2 = new Char_Point(6, player.getPosition().y, colour.BOTH);


        Character marine1 = null;
        Character marine2 = null;
        List<Character> yourPlayers = whichOne>0 ? userPlayers : computerPlayers;
        int castle_y = 0;

        if (movePos.x == castle_1.x && movePos.y == castle_1.y)
        {
                marine1 = yourPlayers[0];
                marine2 = yourPlayers[1];
                castle_y = 2;
        }
        else if (movePos.x == castle_2.x && movePos.y == castle_2.y)
        {
            marine1 = yourPlayers[12];
            marine2 = yourPlayers[13];
            castle_y = 5;
        }
        else
        {
            return;
        }

        castle_move.Add(marine1);
        castle_move.Add(marine2);

        stat_change.Add(new moveStatus
        (
            marine1,
            marine1.getPosition(),
            marine1.getIsFirstMove(),
            State.ALIVE
        ));

        stat_change.Add(new moveStatus
        (
            marine2,
            marine2.getPosition(),
            marine2.getIsFirstMove(),
            State.ALIVE
        ));

        marine1.setPosition(new Char_Point(castle_y, marine1.getPosition().y, marine1.getPosition().colour));
        marine2.setPosition(new Char_Point(castle_y, marine2.getPosition().y, marine2.getPosition().colour));


    }
    public List<Char_Point> availableOfLastRow(Character coastGuard, colour colour)
    {
        List<Char_Point> result = new List<Char_Point>();

        var yourPosition = coastGuard.getOwner() == Owner.USER ? userPlayersPosition : computerPlayersPosition;
        var opponentPosition = coastGuard.getOwner() == Owner.USER ? computerPlayersPosition : userPlayersPosition;
        int y = coastGuard.getInitPos().y == 0 ?  0: Character.BOARD_HEIGHT;


        //var yourPosition = coastGuard.getOwner() == Owner.USER ? computerPlayersPosition : userPlayersPosition;
        //var opponentPosition = coastGuard.getOwner() == Owner.USER ? userPlayersPosition : computerPlayersPosition;
        //int y = coastGuard.getInitPos().y == 1 ? Character.BOARD_HEIGHT : 0;

        for (int x = 0; x <= Character.BOARD_WIDTH; x++)
        {
            Char_Point check_point = new Char_Point(x, y, colour);

            if (Character.CheckIfCanMoveTo(check_point, yourPosition, opponentPosition, projectitlePosition))
            {
                result.Add(check_point);
            }
        }

        return result;

    }
    public void cleanUpResuePostion()
    {
        available_promote_point.Clear();
        rescuecolour.Clear();
    }

    public Character getPlayerByName(CharacterDef def, List<Character> listPlayers)
    {
        var player = listPlayers.FirstOrDefault<Character>(p => p.getName() == def);
        return player;
    }
    public bool isPresidentInCheck(int whichOne)
    {
        Character president = whichOne>0 ? getPlayerByName(CharacterDef.PRESIDENT, userPlayers) : getPlayerByName(CharacterDef.PRESIDENT, computerPlayers);
        HashSet<int> couldBeKilled = president.getOwner() == Owner.USER ? couldBeKilledByComp : couldBeKilledByUser;

        if (president.isAlive() != State.ALIVE)
            return false;

        if (couldBeKilled.Contains(president.getPosition().index()))
            return true;

        return false;

    }
    public GameState isGameOver()
    {
        Character userPresident = getPlayerByName(CharacterDef.PRESIDENT, userPlayers);
        Character compPresident = getPlayerByName(CharacterDef.PRESIDENT, computerPlayers);

        if (userPresident.isAlive() != State.ALIVE || !isValidMoveAvailable(1))
            return GameState.COMP_WIN;

        if (compPresident.isAlive() != State.ALIVE || !isValidMoveAvailable(0))
            return GameState.USER_WIN;

        int alivePieces = 0;
        foreach (var player in userPlayers)
        {
            if (player.isAlive() == State.ALIVE)
                alivePieces++;
        }

        if (alivePieces <= 1)
            return GameState.COMP_WIN;

        alivePieces = 0;
        foreach (var player in computerPlayers)
        {
            if (player.isAlive() == State.ALIVE)
                alivePieces++;
        }

        if (alivePieces <= 1)
            return GameState.USER_WIN;

        return GameState.ONGOING;

    }
    public bool isValidMoveAvailable(int whichOne)
    {
        List<Character> players = whichOne>0 ? userPlayers : computerPlayers;
        List<Char_Point> yourPlayerPosition = whichOne>0 ? userPlayersPosition : computerPlayersPosition;
        List<Char_Point> opponentPlayerPosition = whichOne>0 ? computerPlayersPosition : userPlayersPosition;
        HashSet<int> couldBeKilled = whichOne>0 ? couldBeKilledByComp : couldBeKilledByUser;

        for (int i = 0; i < players.Count; i++)
        {
            Character player = players[i];
            if (player.isAlive() == State.ALIVE)
            {
                List<Char_Point> available_moves = player.availableMovePoints(yourPlayerPosition, opponentPlayerPosition, projectitlePosition);
                foreach (Char_Point point in available_moves)
                {
                    var move_result = updatePositionForPlayer(point, whichOne, i);
                    if (move_result.isValid)
                    {
                        // We don't actually want to move, just checking if there's still a valid move
                        unDoAction(1);
                        return true;
                    }
                }
            }
        }

        return false;

    }

    public moveDecission getPlayerAndMovePoint()
    {
        moveDecission result;
        List<Character> canMove = new List<Character>();

        if (isPresidentInCheck(0))
        {
            return rescuePresidentFromCheck();
        }

        if (couter_to_clear_attack_list > 5)
        {
            attack_presidents.Clear();
        }

        result = tryToCaptureUser(canMove);

        if (result.player == null)
        {
            result = tryToAttackUserPresident(canMove);

            if (result.player != null)
            {
                if (attack_presidents.Count > 3)
                {
                    bool is_the_same = true;
                    foreach (var p in attack_presidents)
                    {
                        if (p != result.player)
                        {
                            is_the_same = false;
                        }
                    }

                    if (is_the_same)
                    {
                        result.player = null;
                    }
                    else
                    {
                        attack_presidents.RemoveAt(0);
                        attack_presidents.Add(result.player);
                    }
                }
                else
                {
                    attack_presidents.Add(result.player);
                }
            }
        }
        else
        {
            couter_to_clear_attack_list++;
        }

        if (result.player == null)
        {
            couter_to_clear_attack_list++;
            return getRandomMove(canMove);
        }

        return result;

    }

    public moveDecission getPlayerAndMovePointForPlayerTeamBot()
    {
        moveDecission result;
        List<Character> canMove = new List<Character>();

        if (isPresidentInCheck(1))//1==user in this bot's team
        {
            return rescuePresidentFromCheckPlayerTeamBot();
        }

        if (couter_to_clear_attack_list > 5)
        {
            attack_presidents.Clear();
        }

        result = tryToCaptureBot(canMove);

        if (result.player == null)
        {
            result = tryToAttackBotPresident(canMove);

            if (result.player != null)
            {
                if (attack_presidents.Count > 3)
                {
                    bool is_the_same = true;
                    foreach (var p in attack_presidents)
                    {
                        if (p != result.player)
                        {
                            is_the_same = false;
                        }
                    }

                    if (is_the_same)
                    {
                        result.player = null;
                    }
                    else
                    {
                        attack_presidents.RemoveAt(0);
                        attack_presidents.Add(result.player);
                    }
                }
                else
                {
                    attack_presidents.Add(result.player);
                }
            }
        }
        else
        {
            couter_to_clear_attack_list++;
        }

        if (result.player == null)
        {
            couter_to_clear_attack_list++;
            return getRandomMoveForPlayerTeamBot(canMove);
        }

        return result;

    }

    public moveDecission throwProjectileForComputer()
    {
        moveDecission result = new moveDecission();
        result.is_chip = true;
        List<Character> canMove = new List<Character>();
        Character compPresident = getPlayerByName(CharacterDef.PRESIDENT, computerPlayers);
        Character userPresident = getPlayerByName(CharacterDef.PRESIDENT, userPlayers);

        // Avoid throwing projectiles in the president row/column
        foreach (Character p in computerChips)
        {
            if (p.isAlive() != State.ALIVE || p.getPosition().index() != p.getInitPos().index())
                continue;

            bool ok_to_pick = true;
            bool should_pick = false;
            List<Char_Point> availableMove = p.availableMovePoints(computerPlayersPosition, userPlayersPosition, projectitlePosition);

            foreach (Char_Point point in availableMove)
            {
                if (point.index() == compPresident.getPosition().index())
                {
                    // Might kill your president. Don't do that.
                    ok_to_pick = false;
                    break;
                }

                if (point.index() == userPresident.getPosition().index())
                {
                    // Should do, you might kill opponent's president
                    should_pick = true;
                }

                if (userPlayersPosition.Contains(point))
                {
                    should_pick = true;
                }
            }

            if (ok_to_pick && should_pick)
            {
                // Take your chance to kill their opponent's president
                result.player = p;
                result.available_move = availableMove;
                result.moveTo = result.available_move[UnityEngine.Random.Range(0, result.available_move.Count)];
                return result;
            }

            if (ok_to_pick)
                canMove.Add(p);
        }

        // Pick a random move in the good direction
        if (canMove.Count > 0)
        {
            try
            {
                Debug.Log("TEST::CanMove Count::"+canMove.Count);
                int random_index = UnityEngine.Random.Range(0, canMove.Count);
                Debug.Log("TEST::random_index::" + random_index);
                result.player = canMove[random_index];
                result.available_move = result.player.availableMovePoints(computerPlayersPosition, userPlayersPosition, projectitlePosition);
                Debug.Log("TEST::result.available_move::" + result.available_move.Count);
                random_index = UnityEngine.Random.Range(0, result.available_move.Count);
                Debug.Log("TEST::random_index::" + random_index);
                result.moveTo = result.available_move[random_index];
                Debug.Log("TEST::Pick projectile and move to " + result.moveTo.index());
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log(e.ToString());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e.ToString());
            }
            
        }

        return result;

    }

    public moveDecission throwProjectileForComputerPlayerTeamBot()
    {
        moveDecission result = new moveDecission();
        result.is_chip = true;
        List<Character> canMove = new List<Character>();
        Character compPresident = getPlayerByName(CharacterDef.PRESIDENT, computerPlayers);
        Character userPresident = getPlayerByName(CharacterDef.PRESIDENT, userPlayers);

        // Avoid throwing projectiles in the president row/column
        foreach (Character p in userChips)
        {
            if (p.isAlive() != State.ALIVE || p.getPosition().index() != p.getInitPos().index())
                continue;

            bool ok_to_pick = true;
            bool should_pick = false;
            List<Char_Point> availableMove = p.availableMovePoints(userPlayersPosition, computerPlayersPosition, projectitlePosition);

            foreach (Char_Point point in availableMove)
            {
                if (point.index() == userPresident.getPosition().index())
                {
                    // Might kill your president. Don't do that.
                    ok_to_pick = false;
                    break;
                }

                if (point.index() == compPresident.getPosition().index())
                {
                    // Should do, you might kill opponent's president
                    should_pick = true;
                }

                if (computerPlayersPosition.Contains(point))
                {
                    should_pick = true;
                }
            }

            if (ok_to_pick && should_pick)
            {
                // Take your chance to kill their opponent's president
                result.player = p;
                result.available_move = availableMove;
                result.moveTo = result.available_move[UnityEngine.Random.Range(0, result.available_move.Count)];
                return result;
            }

            if (ok_to_pick)
                canMove.Add(p);
        }

        // Pick a random move in the good direction
        if (canMove.Count > 0)
        {
            try
            {
                Debug.Log("TEST::CanMove Count::" + canMove.Count);
                int random_index = UnityEngine.Random.Range(0, canMove.Count);
                Debug.Log("TEST::random_index::" + random_index);
                result.player = canMove[random_index];
                result.available_move = result.player.availableMovePoints(userPlayersPosition, computerPlayersPosition, projectitlePosition);
                Debug.Log("TEST::result.available_move::" + result.available_move.Count);
                random_index = UnityEngine.Random.Range(0, result.available_move.Count);
                Debug.Log("TEST::random_index::" + random_index);
                result.moveTo = result.available_move[random_index];
                Debug.Log("TEST::Pick projectile and move to " + result.moveTo.index());
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log(e.ToString());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e.ToString());
            }

        }

        return result;

    }

    public moveDecission getPromoteForComp()
    {
        moveDecission result = new moveDecission();

        foreach (var player in computerPlayers)
        {
            if (player.isAlive() != State.CAPTURE)
                continue;

            if (rescuecolour.Contains(player.getPosition().colour))
            {
                result.player = player;
                result.available_move = available_promote_point.Where(aPoint => result.player.getPosition().colour == aPoint.colour).ToList();

                int random_index = UnityEngine.Random.Range(0, result.available_move.Count);
                result.moveTo = result.available_move[random_index];
                break;
            }
        }

        return result;

    }

    public moveDecission getPromoteForCompPlayerTeamBot()
    {
        moveDecission result = new moveDecission();

        foreach (var player in userPlayers)
        {
            if (player.isAlive() != State.CAPTURE)
                continue;

            if (rescuecolour.Contains(player.getPosition().colour))
            {
                result.player = player;
                result.available_move = available_promote_point.Where(aPoint => result.player.getPosition().colour == aPoint.colour).ToList();

                int random_index = UnityEngine.Random.Range(0, result.available_move.Count);
                result.moveTo = result.available_move[random_index];
                break;
            }
        }

        return result;

    }
}
