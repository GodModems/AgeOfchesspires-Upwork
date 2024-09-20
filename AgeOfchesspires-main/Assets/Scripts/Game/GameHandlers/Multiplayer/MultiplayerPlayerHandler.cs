using Fusion;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.CullingGroup;

public enum MultiplayerGameState { ONGOING, USER_WIN, OPP_WIN }
public enum MultiplayerTurnState { PLAYER_TURN, OPP1_TURN, OPP2_TURN, FRND_OPP_TURN };



public class MultiplayerPlayerHandler : MonoBehaviour
{
    private static PlayerHandler pInstance;
    private List<Char_Point> userPlayersPosition = new List<Char_Point>();
    private List<Char_Point> oppPlayersPosition = new List<Char_Point>();
    private List<Char_Point> projectitlePosition = new List<Char_Point>();
    private List<Character> userPlayers = new List<Character>();
    private List<Character> oppPlayers = new List<Character>();
    private List<Character> userChips = new List<Character>();
    private List<Character> oppChips = new List<Character>();
    private HashSet<int> couldBeKilledByOpp = new HashSet<int>();
    private HashSet<int> couldBeKilledByUser = new HashSet<int>();
    //private TurnState _turnState;
    //public Stack<MultiplayerTurnState> turnStateSeqnce = new Stack<MultiplayerTurnState>();

    

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
    private float user_point=0;
    private float opp_point=0;
    private float opp2_point = 0;
    private float opp3_point = 0;
    private bool isSwitchPresidentPostion;
    private bool isGameTypeChance;
    private bool isBlack;

    private string encodeCharPoint(Char_Point p)
    {
        List<string> column16_ref = null;
        List<string> row16_ref = null;
        List<string> column8_ref = null;
        List<string> row8_ref = null;

        string res = "";

        bool is_rot = FindObjectOfType<MultiplayerGamePlay>().GetIsRotated();

        if (is_rot)
        {
            //column16_ref = 
            //    new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p" };
            //row16_ref = 
            //    new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };
            column8_ref = new List<string> { "h", "g", "f", "e", "d", "c", "b", "a" };
            row8_ref = 
                new List<string> { "8", "7", "6", "5", "4", "3", "2", "1" };
        }
        else
        {
            //column16_ref = 
            //    new List<string> { "p", "o", "n", "m", "l", "k", "j", "i", "h", "g", "f", "e", "d", "c", "b", "a" };
           // row16_ref = 
            //    new List<string> { "16", "15", "14", "13", "12", "11", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" };
            column8_ref = 
                new List<string> { "a", "b", "c", "d", "e", "f", "g", "h" };
            row8_ref = 
                new List<string> { "1", "2", "3", "4", "5", "6", "7", "8" };

            

        }



        //List<string> row16 = new List<string>(row16_ref);
        //List<string> column16 = new List<string>(column16_ref);
        List<string> row8 = new List<string>(row8_ref);
        List<string> column8 = new List<string>(column8_ref);

   

        if (p.x < 0 || p.x > 7 || p.y < 0 || p.y > 7)
            res = "0";

        //if (p.colour == colour.BOTH)
            res = column8[p.y] + row8[p.x];
        /*else if (p.colour == colour.BLACK)
        {
            string row = p.x % 2 == 0 ? row16[p.x * 2] : row16[p.x * 2 + 1];
            string column = p.y % 2 == 0 ? column16[p.y * 2 + 1] : column16[p.y * 2];
            res = column + row;

        }
        else
        {
            string row = p.x % 2 == 0 ? row16[p.x * 2 + 1] : row16[p.x * 2];
            string column = p.y % 2 == 0 ? column16[p.y * 2] : column16[p.y * 2 + 1];
            res = column + row;

        }*/

        return res;

    }
    private void checkRescueAbilitycolour(Character coastGuard)
    {
        var yourPlayers = coastGuard.getOwner() == Owner.USER ? userPlayers : oppPlayers;


        foreach (var p in yourPlayers)
        {
            if (p.isAlive() == State.CAPTURE)
            {
                Debug.Log("Rescue:Adding my player to be rescued!");
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
        oppPlayersPosition.Clear();
        projectitlePosition.Clear();
        foreach (var p in userPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                userPlayersPosition.Add(p.getPosition());
            }
        }

        foreach (var p in oppPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                oppPlayersPosition.Add(p.getPosition());
            }
        }

        foreach (var p in userChips)
        {
            if (p.isAlive() == State.ALIVE && Character.IsValidPoint(p.getPosition()))
            {
                projectitlePosition.Add(p.getPosition());
            }
        }

        foreach (var p in oppChips)
        {
            if (p.isAlive() == State.ALIVE && Character.IsValidPoint(p.getPosition()))
            {
                projectitlePosition.Add(p.getPosition());
            }
        }

    }

    

    public void cleanupForNewScene()
    {
        userPlayersPosition.Clear();
        oppPlayersPosition.Clear();
        projectitlePosition.Clear();
        encode_games.Clear();
        couldBeKilledByOpp.Clear();
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

        foreach (var player in oppPlayers)
        {
            player.Dispose();
        }
        oppPlayers.Clear();

        foreach (var chip in userChips)
        {
            chip.Dispose();
        }
        userChips.Clear();

        foreach (var chip in oppChips)
        {
            chip.Dispose();
        }
        oppChips.Clear();

    }
    public float getPoint(int whichOne) 
    { 
      
        if(whichOne == 1)
        {
            return user_point;
        }
        else if (whichOne == 0)
        {
            return opp_point;
        }
        else if (whichOne == 2)
        {
            return opp2_point;
        }
        else if (whichOne == 3)
        {
            Debug.Log("MA::Get Point Comp3::" + opp3_point);

            return opp3_point;
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
            indexForButton = oppPlayers[index].getPosition().index();

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
        result = encode_games.Last().ToString();//i want just last step to be returned

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
    public List<Character> getOppPlayers() { return oppPlayers; }
    public List<Character> getUserChips() { return userChips; }
    public List<Character> getOppChips() { return oppChips; }
    public List<Char_Point> selectWhereToGo(int index, int whichone)
    {
            List<Char_Point> empty = new List<Char_Point>();
            isPosibleCastle = false;
            Character player;
            HashSet<int> couldBeKilled = whichone > 0 ? couldBeKilledByOpp : couldBeKilledByUser;
            List<Char_Point> yourPlayerPosition = whichone>0 ? userPlayersPosition : oppPlayersPosition;
            List<Char_Point> opponentPlayerPosition = whichone>0 ? oppPlayersPosition : userPlayersPosition;

            // Click on Projectile
            if (index >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
            {
                player = index >= (int)GameConstants.PlayerTags.COMP_CHIP_TAG ? oppChips[index - (int)GameConstants.PlayerTags.COMP_CHIP_TAG] : userChips[index - (int)GameConstants.PlayerTags.USER_CHIP_TAG];
                if (player.isAlive() == State.ALIVE)
                    return player.availableMovePoints(yourPlayerPosition, opponentPlayerPosition, projectitlePosition);
                else
                    return empty;
            }

        //Debug.Log("Index2::" + index);
        //Debug.Log("WhichOne::" + whichone);
        //Debug.Log("UserPlayers Count::" + userPlayers.Count);
        //Debug.Log("OppPlayers Count::" + oppPlayers.Count);

        // Click on Player
        player = whichone> 0 ? userPlayers[index] : oppPlayers[index];

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
            var player = whichone>0 ? userPlayers[index] : oppPlayers[index];

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
    public updatePosStatus updatePositionForPlayer(Char_Point movePos, int whichOne, int index, bool isSync = false, bool isCastleSync = false, int isPromoted = -1)
    {
        updatePosStatus result = new updatePosStatus();
        var opponentPlayers = whichOne>0 ? oppPlayers : userPlayers;
        var player = whichOne>0 ? userPlayers[index] : oppPlayers[index];

        Debug.Log("->Updated Position Character, isPromoted:"+isPromoted);

        if (!isSync)
        {

            // changeTracking;
            List<moveStatus> stat_change = new List<moveStatus>();

            stat_change.Add(new moveStatus(player, player.getPosition(), player.getIsFirstMove(), player.isAlive()));

            for (int i = 0; i < opponentPlayers.Count; i++)
            {

                if (checkCaptureKillPlayer(player, opponentPlayers[i], movePos))
                {
                    Debug.Log("MA::CaptureOrKillPlayer True:i=" + i);
                    stat_change.Add(new moveStatus(opponentPlayers[i], opponentPlayers[i].getPosition(), opponentPlayers[i].getIsFirstMove(), State.ALIVE));
                    result.removePlayers.Add(i);

                    //Haitian when capturing::OK
                    if (GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv
                        == (int)GameConstants.Civilisation.Haitians)
                    {
                        //Debug.Log("FOR HAITIANS::I AM HAITIAN!!!");
                        if (player.getName() == CharacterDef.NAVY)
                        {
                            // i need to see what piece i am capturing
                            // i  will be transformed to that piece
                            //Debug.Log("FOR HAITIANS::WITCH HAVE CAPTURED!!!" + opponentPlayers[i].getName());

                            if (opponentPlayers[i].getName() != CharacterDef.PRESIDENT)
                            {
                                player.SetPromoted(true);
                                Debug.Log("Set Conversion True:option" + opponentPlayers[i].getName());

                                player.SetPromotedOption((int)opponentPlayers[i].getName());

                                MultiplayerGamePlay multiplayerGamePlay = GameObject.FindObjectOfType<MultiplayerGamePlay>();


                                indexPlayer tag = getindexByCharacter(player);

                                Button btn = multiplayerGamePlay.getButtonByTag(multiplayerGamePlay.GetUserCharBtns(), tag.index);
                                btn.GetComponent<GridInfo>().updateSprite((int)opponentPlayers[i].getName());
                            }
                        }
                    }
                    else if(GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv
                        == (int)GameConstants.Civilisation.Britons && player.getName() == CharacterDef.COASTGUARD)
                    {
                        MultiplayerGamePlay multiplayerGamePlay = GameObject.FindObjectOfType<MultiplayerGamePlay>();
                        indexPlayer tag = getindexByCharacter(player);

                        Button btn = multiplayerGamePlay.getButtonByTag(multiplayerGamePlay.GetUserCharBtns(), tag.index);
                        
                        GridInfo gridInfo = btn.GetComponent<GridInfo>();
                        movePos = new Char_Point(gridInfo.x, gridInfo.y, colour.BOTH);
                    }


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
                Debug.Log("Rescue:I am at the end:");

                player.setPosition(movePos);
                updateStatusAfterMove();
                checkRescueAbilitycolour(player);
                if (rescuecolour.Count == 0)
                {
                    isWaitingForPromote = true;
                }
                else
                {
                    isWaitingForPromote = false;
                }
            }*/


            // for rescue the when Coast-Guard go to the end.
            //if (player.isAlive() == State.CAPTURE)
            //{
            //    rescuecolour.Clear();
            //    available_promote_point.Clear();
            //    //player.SetAlive(State.ALIVE);

            //    encode_games[encode_games.Count - 1] += /*player.getEncodeName() + "" +*/ encodeCharPoint(player.getPosition()) + "-" + encodeCharPoint(movePos);

            //}
            //else
            //{
            

                if (result.removePlayers.Count > 0)
                    encode_games.Add(/*player.getEncodeName() + "" +*/ encodeCharPoint(player.getPosition()) + "-" + encodeCharPoint(movePos));
                else
                    encode_games.Add(/*player.getEncodeName() + "" +*/ encodeCharPoint(player.getPosition()) + "-" + encodeCharPoint(movePos));
            //}

            player.setPosition(movePos);
            player.setIsFirstMove(false);
            changeTracking.Push(stat_change);
            updateStatusAfterMove();
            isPosibleCastle = false;

            //Haitian when not capturing but need to check its adjacent and diagonal to convert
            if (GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv
                == (int)GameConstants.Civilisation.Haitians)
            {
                Debug.Log("FOR HAITIANS::I AM HAITIAN!!!++++++++++++++++++");

                if (player.getName() == CharacterDef.NAVY)
                {
                    List<Character> oppPlayersFound = CheckAdjacentPositionsForWitch(1, movePos, oppPlayers);
                    if (oppPlayersFound.Count() > 0)
                    {
                        Debug.Log("Opp Players found around witch:"+oppPlayersFound.Count());
                        foreach (Character oppPlayer in oppPlayersFound)
                        {
                            //convert this opponent piece to my pawn
                            //Debug.Log("FOR HAITIANS::I MOVED AT ADJACENT OR DIAGONAL, CONVERT THIS OPPONENT TO MY PAWN!!!");
                            MultiplayerGamePlay multiplayerGamePlay = GameObject.FindObjectOfType<MultiplayerGamePlay>();

                            oppPlayer.SetPromoted(true);
                            oppPlayer.SetPromotedOption((int)CharacterDef.COASTGUARD);
                            indexPlayer tag = getindexByCharacter(oppPlayer);

                            Button btn = multiplayerGamePlay.getButtonByTag(multiplayerGamePlay.GetOppCharBtns(), tag.index);
                            btn.GetComponent<GridInfo>().updateSprite((int)CharacterDef.COASTGUARD, true);
                            btn.GetComponent<DragAndDropHandler>().enabled = true;
                            oppPlayer.setOwner(Owner.USER);
                            userPlayers.Add(oppPlayer);
                            opponentPlayers.Remove(oppPlayer);
                            //RoomPlayer.Local.RPC_SyncPostMutation(tag.index);
                        }

                    }
                    else
                    {
                        Debug.Log("FOR HAITIANS::NO OPP AT ADJACENT OR DIAGONAL, SO NO CONVERT THIS OPPONENT TO MY PAWN!!!");

                    }
                }
            }

            

            Debug.Log("@@updating pow and die list");
            GameObject.FindObjectOfType<MultiplayerGamePlay>().UpdatePlayerPowAndDieList(false, true);

            //MA::
            /*if (isPresidentInCheck(whichOne))
            {
                result.isValid = false;
                unDoAction(1);
            }*/



        }
        else
        {
            /*for (int i = 0; i < opponentPlayers.Count; i++)
            {
                checkCaptureKillPlayer(player, opponentPlayers[i], movePos);
            }

            if (player.isAlive() == State.CAPTURE)
            {
                player.SetAlive(State.ALIVE);
            }*/

            player.setPosition(movePos);
            player.SetPromoted(isPromoted>0);

            updateStatusAfterMove();
        }

        return result;

    }

    /*public void SyncUpdatedMovePos(Char_Point movePos, int whichOne, int index)
    {
       
        var player = whichOne > 0 ? userPlayers[index] : oppPlayers[index];
        player.setPosition(movePos);
        
    }*/

    public List<Character> CheckAdjacentPositionsForWitch(int option, Char_Point capturedOppPos, List<Character> opponentPlayers)
    {
        Debug.Log("FOR HAITIANS::CheckAdjacentPositionsForWitch");

        List<Character> resList = new List<Character>();

        // Define the directions to check: up, down, left, right
        List<Char_Point> directions = new List<Char_Point>
        {
            new Char_Point(0, 1),  // Up
            new Char_Point(0, -1), // Down
            new Char_Point(-1, 0), // Left
            new Char_Point(1, 0)   // Right
        };

        if(option == 1)
        {
            directions.Add(new Char_Point(1,1));
            directions.Add(new Char_Point(-1, 1));
            directions.Add(new Char_Point(-1,-1));
            directions.Add(new Char_Point(1,-1));
        }

        // Check each direction
        foreach (Char_Point dir in directions)
        {
            Char_Point adjacentPos = new Char_Point(capturedOppPos.x + dir.x, capturedOppPos.y + dir.y);

            Debug.Log(adjacentPos.x+"FOR HAITIANS::Current Adjacent Pos::"+adjacentPos.y);

            // Check if the position is within the grid bounds
            if (adjacentPos.x >= 0 && adjacentPos.x < 8 && adjacentPos.y >= 0 && adjacentPos.y < 8)
            {
                if (option == 0)
                {
                    foreach (Character opponentPlayer in opponentPlayers)
                    {
                        // Check if there's a Witch at the adjacent position in opponentPlayers
                        if (IsWitchAtPosition(adjacentPos, opponentPlayer))
                        {
                            Debug.Log("FOR HAITIANS::Witch found at position: " + adjacentPos.x + ", " + adjacentPos.y);
                            // Add your logic here for when a Witch is found
                            resList.Add(opponentPlayer);
                        }
                    }
                }
                else if(option == 1)
                {
                    foreach (Character opponentPlayer in opponentPlayers)
                    {
                        // Check if there's a Witch at the adjacent position in opponentPlayers
                        if (IsOpponentAtPosition(adjacentPos, opponentPlayer))
                        {
                            Debug.Log("FOR HAITIANS::Opp found at position: " + adjacentPos.x + ", " + adjacentPos.y);
                            // Add your logic here for when a Witch is found
                            resList.Add(opponentPlayer);
                        }
                    }
                }
            }
        }

        return resList;
    }

    bool IsWitchAtPosition(Char_Point pos, Character player)
    {
        Debug.Log("FOR HAITIANS::IsWitchAtPosition: " + pos.x + ", " + pos.y);

        if ((player.getPosition().x == pos.x && player.getPosition().y == pos.y) && player.getName() == CharacterDef.NAVY)
        {
            return true;
        }
        
        return false;
    }

    bool IsOpponentAtPosition(Char_Point pos, Character player)
    {
        Debug.Log("FOR HAITIANS::IsOpponentAtPosition: " + pos.x + ", " + pos.y);

        if (player.getPosition().x == pos.x && player.getPosition().y == pos.y)
        {
            return true;
            
        }
        return false;
    }


    public updatePosStatus updatePositionForChip(Char_Point movePos, int whichOne, int index, bool isSync=false)
    {

        updatePosStatus result = new updatePosStatus();

        Character chip;
        List<moveStatus> stat_change = new List<moveStatus>();
        if (index >= (int)GameConstants.PlayerTags.COMP_CHIP_TAG)
        {
            chip = oppChips[index - (int)GameConstants.PlayerTags.COMP_CHIP_TAG];
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

        if (!isSync)
        {

            stat_change.Add(new moveStatus(chip, chip.getPosition(), chip.getIsFirstMove(), chip.isAlive()));

            for (int i = 0; i < userPlayers.Count; i++)
            {
                if (checkCaptureKillPlayer(chip, userPlayers[i], movePos))
                {
                    stat_change.Add(new moveStatus(userPlayers[i], userPlayers[i].getPosition(), userPlayers[i].getIsFirstMove(), State.ALIVE));
                    result.removePlayers.Add(i);
                }
            }

            for (int i = 0; i < oppPlayers.Count; i++)
            {
                if (checkCaptureKillPlayer(chip, oppPlayers[i], movePos))
                {
                    stat_change.Add(new moveStatus(oppPlayers[i], oppPlayers[i].getPosition(), oppPlayers[i].getIsFirstMove(), State.ALIVE));
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

            for (int i = 0; i < oppChips.Count; i++)
            {
                if (Character.IsValidPoint(oppChips[i].getPosition()) && checkCaptureKillPlayer(chip, oppChips[i], movePos))
                {
                    stat_change.Add(new moveStatus(oppChips[i], oppChips[i].getPosition(), oppChips[i].getIsFirstMove(), State.ALIVE));
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

                //Multiplayer RPC

                //Char_Point candidateChip = new Char_Point();
                int cx = 7 - chip.getPosition().x;
                int cy = 7 - chip.getPosition().y;
                int cColour = (int)chip.getPosition().colour;

                //Char_Point candidate = new Char_Point();
                int px = 7 - movePos.x;
                int py = 7 - movePos.y;
                int pColour = (int)movePos.colour;
                uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;

                RoomPlayer.Local.RPC_SetMovement(localId, cx, cy, cColour, px, py, pColour, true, false, -1);
            }
            changeTracking.Push(stat_change);
            updateStatusAfterMove();
        }
        else
        {
            chip.setPosition(movePos);
            updateStatusAfterMove();
        }

        return result;

    }
    public updatePosStatus updatePosition(Char_Point movePos, int whichOne, int index)
    {
        castle_move.Clear();
        //if (index >= (int)GameConstants.PlayerTags.USER_CHIP_TAG)
        //    return updatePositionForChip(movePos, whichOne, index);
        //else
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
        for (int i = 0; i < oppPlayers.Count; i++)
        {
            if (oppPlayers[i] == player)
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

        for (int i = 0; i < oppChips.Count; i++)
        {
            if (oppChips[i] == player)
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
            chip = oppChips[tag - (int)GameConstants.PlayerTags.COMP_CHIP_TAG];

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
        couldBeKilledByOpp.Clear();
        couldBeKilledByUser.Clear();
        foreach (var p in userPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                var canKill = p.pointCanKillOpponent(userPlayersPosition, oppPlayersPosition, projectitlePosition);
                foreach (var pos in canKill)
                {
                    if (pos.index() >= 0 && pos.index() <= 77)
                        couldBeKilledByUser.Add(pos.index());
                }
            }
        }

        foreach (var p in oppPlayers)
        {
            if (p.isAlive() == State.ALIVE)
            {
                var canKill = p.pointCanKillOpponent(oppPlayersPosition, userPlayersPosition, projectitlePosition);
                foreach (var pos in canKill)
                {
                    if (pos.index() >= 0 && pos.index() <= 77)
                        couldBeKilledByOpp.Add(pos.index());
                }
            }
        }

    }

    public void updatePoint()
    {
        user_point = 0;
        opp_point = 0;
        opp2_point = 0;
        opp3_point = 0;
        foreach (var p in userPlayers)
        {
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && !p.isOutByComp2())
            {
                opp_point += p.getValue();
                
                
            }
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && p.isOutByComp2())
            {
                opp2_point += p.getValue();
            }
        }

        

        foreach (var p in oppPlayers)
        {
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && p.isOutByComp3())
            {
                Debug.Log("IS out by Comp3::");
                opp3_point += p.getValue();
            }
            if ((p.isAlive() == State.CAPTURE || p.isAlive() == State.DIED) && !p.isOutByComp3())
            {
                Debug.Log("IS out by User::");
                user_point += p.getValue();
            }
            
        }

      
    }

    public void setSwitchPresidentPos(bool x) 
    { 
        isSwitchPresidentPostion = x;
    }

    public void setIsGameTypeChance(bool x)
    {
        isGameTypeChance = x;
    }

    public void initBoardData(bool isBlack)
    {
        int val = PlayerPrefs.GetInt("switchChar" + RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw, 1);
        Debug.Log("POS::Init board data::switchChar::"+val);
        bool isSwitchPresidentPostion = false;
        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;

        int myPawnCount = 8;
        int myKnightCount = 2;

        // User Players
        userPlayers.Add(createCharacter(new Char_Point(0, 0, colour.BOTH), CharacterDef.MARINE, Owner.USER));

        if ((int)GameConstants.Civilisation.Egyptian == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(1, 0, colour.BOTH), CharacterDef.AIRFORCE, Owner.USER));
        }
        else if ((int)GameConstants.Civilisation.Huns == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(1, 0, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));

        }
        else
        {
            userPlayers.Add(createCharacter(new Char_Point(1, 0, colour.BOTH), CharacterDef.NAVY, Owner.USER));
        }

        if ((int)GameConstants.Civilisation.Spanish == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(2, 0, colour.BOTH), CharacterDef.NAVY, Owner.USER));
            myKnightCount += 1;

        }
        else if ((int)GameConstants.Civilisation.Huns == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(2, 0, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));

        }
        else
        {
            userPlayers.Add(createCharacter(new Char_Point(2, 0, colour.BOTH), CharacterDef.AIRFORCE, Owner.USER));
        }

        Debug.Log("Switch pos bool:"+isSwitchPresidentPostion);

        if ((int)GameConstants.Civilisation.French == myCiv)
        {
            Debug.Log("Switch pos false");
            userPlayers.Add(createCharacter(new Char_Point(4, 0, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(3, 0, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));
            myPawnCount = 10;
        }
        else if ((int)GameConstants.Civilisation.Egyptian == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(4, 0, colour.BOTH), CharacterDef.AIRFORCE, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(3, 0, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
        }
        else if ((int)GameConstants.Civilisation.Spanish == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(4, 0, colour.BOTH), CharacterDef.ARMY, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(3, 0, colour.BOTH), CharacterDef.NAVY, Owner.USER));
            myKnightCount += 1;
        }
        else if ((int)GameConstants.Civilisation.Huns == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(4, 0, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(3, 0, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
        }
        else
        {
            userPlayers.Add(createCharacter(new Char_Point(4, 0, colour.BOTH), CharacterDef.ARMY, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(3, 0, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
        }
        

        GameObject.FindObjectOfType<MultiplayerGamePlay>().myPawnCount = myPawnCount;

        if ((int)GameConstants.Civilisation.Spanish == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(5, 0, colour.BOTH), CharacterDef.NAVY, Owner.USER));
            myKnightCount += 1;
        }
        else if ((int)GameConstants.Civilisation.Huns == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(5, 0, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));

        }
        else
        {
            userPlayers.Add(createCharacter(new Char_Point(5, 0, colour.BOTH), CharacterDef.AIRFORCE, Owner.USER));
        }

        GameObject.FindObjectOfType<MultiplayerGamePlay>().myKnightCount = myKnightCount;


        if ((int)GameConstants.Civilisation.Egyptian == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(6, 0, colour.BOTH), CharacterDef.AIRFORCE, Owner.USER));
        }
        else if ((int)GameConstants.Civilisation.Huns == myCiv)
        {
            userPlayers.Add(createCharacter(new Char_Point(6, 0, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));

        }
        else
        {
            userPlayers.Add(createCharacter(new Char_Point(6, 0, colour.BOTH), CharacterDef.NAVY, Owner.USER));
        }

        userPlayers.Add(createCharacter(new Char_Point(7, 0, colour.BOTH), CharacterDef.MARINE, Owner.USER));

        for (int i = 0; i < 8; i++)
        {
            userPlayers.Add(createCharacter(new Char_Point(i, 1, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));
            if ((int)GameConstants.Civilisation.Huns == myCiv)
            {
            }
        }

        if ((int)GameConstants.Civilisation.Chinese == myCiv)
        {
            for (int i = 2; i < 6; i++)
            {
                userPlayers.Add(createCharacter(new Char_Point(i, 2, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));
            }
        }

        Debug.Log("MA::"+userPlayers.Count);

        int oppCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currOppCiv;
        int oppPawnCount = 8;
        int oppKnightCount = 2;

        // Computer Players
        oppPlayers.Add(createCharacter(new Char_Point(0, 7, colour.BOTH), CharacterDef.MARINE, Owner.COMPUTER));

        if ((int)GameConstants.Civilisation.Egyptian == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(1, 7, colour.BOTH), CharacterDef.AIRFORCE, Owner.COMPUTER));
        }
        else if ((int)GameConstants.Civilisation.Huns == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(1, 7, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));

        }
        else
        {
            oppPlayers.Add(createCharacter(new Char_Point(1, 7, colour.BOTH), CharacterDef.NAVY, Owner.COMPUTER));
        }


        if ((int)GameConstants.Civilisation.Spanish == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(2, 7, colour.BOTH), CharacterDef.NAVY, Owner.COMPUTER));
            oppKnightCount += 1;
        }
        else if ((int)GameConstants.Civilisation.Huns == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(2, 7, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));

        }
        else
        {

            oppPlayers.Add(createCharacter(new Char_Point(2, 7, colour.BOTH), CharacterDef.AIRFORCE, Owner.COMPUTER));
        }


        if ((int)GameConstants.Civilisation.French == oppCiv)
        {

            oppPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));
            oppPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));

            oppPawnCount = 10;
        }
        else if ((int)GameConstants.Civilisation.Egyptian == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.AIRFORCE, Owner.USER));
            oppPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
        }
        else if ((int)GameConstants.Civilisation.Spanish == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.ARMY, Owner.USER));
            oppPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.NAVY, Owner.USER));
            oppKnightCount += 1;
        }
        else if ((int)GameConstants.Civilisation.Huns == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));
            oppPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.PRESIDENT, Owner.COMPUTER));
        }
        else
        {
            oppPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.ARMY, Owner.COMPUTER));
            oppPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.PRESIDENT, Owner.COMPUTER));
        }



        if ((int)GameConstants.Civilisation.Spanish == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(5, 7, colour.BOTH), CharacterDef.NAVY, Owner.COMPUTER));
            oppKnightCount += 1;
        }
        else if ((int)GameConstants.Civilisation.Huns == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(5, 7, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));

        }
        else
        {
            oppPlayers.Add(createCharacter(new Char_Point(5, 7, colour.BOTH), CharacterDef.AIRFORCE, Owner.COMPUTER));

        }

        if ((int)GameConstants.Civilisation.Egyptian == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(6, 7, colour.BOTH), CharacterDef.AIRFORCE, Owner.COMPUTER));

        }
        else if ((int)GameConstants.Civilisation.Huns == oppCiv)
        {
            oppPlayers.Add(createCharacter(new Char_Point(6, 7, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));

        }
        else
        {
            oppPlayers.Add(createCharacter(new Char_Point(6, 7, colour.BOTH), CharacterDef.NAVY, Owner.COMPUTER));

        }
        oppPlayers.Add(createCharacter(new Char_Point(7, 7, colour.BOTH), CharacterDef.MARINE, Owner.COMPUTER));


        for (int i = 0; i < 8; i++)
        {
            oppPlayers.Add(createCharacter(new Char_Point(i, 6, colour.BOTH), CharacterDef.COASTGUARD, Owner.COMPUTER));
            if ((int)GameConstants.Civilisation.Huns == oppCiv)
            {
            }
        }

        if ((int)GameConstants.Civilisation.Chinese == oppCiv)
        {
            for (int i = 2; i < 6; i++)
            {
                oppPlayers.Add(createCharacter(new Char_Point(i, 5, colour.BOTH), CharacterDef.COASTGUARD, Owner.USER));
            }
        }

        this.isBlack = isBlack;
        updateArrayPosition();
        updateStatusAfterMove();

        Debug.Log("MA,userplayers added::" + userPlayers.Count);
        Debug.Log("MA,opp players added::" + oppPlayers.Count);
    }

    public void SyncCharPosOpp(int val1)
    {
        bool isSwitchPresidentPostionOpp = true;

        Debug.Log("POS::Opp Switch Char:" + val1);

        if (isSwitchPresidentPostionOpp)
        {
            oppPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.ARMY, Owner.COMPUTER));
            oppPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.PRESIDENT, Owner.COMPUTER));
            
        }
        else
        {
            oppPlayers.Add(createCharacter(new Char_Point(3, 7, colour.BOTH), CharacterDef.ARMY, Owner.COMPUTER));
            oppPlayers.Add(createCharacter(new Char_Point(4, 7, colour.BOTH), CharacterDef.PRESIDENT, Owner.COMPUTER));
        }
    }

    private int r = 0;

    List<int[]> randPosGOC1 = new List<int[]>
    {
        new int[] {0,2,1,3,4,5,6,7},
        new int[] {0,7,6,3,4,5,1,2},
        new int[] {0,6,5,3,4,7,2,1},
        new int[] {0,2,1,3,4,5,7,6},
        new int[] {2,1,0,3,4,7,6,5},
        new int[] {1,3,0,3,4,6,5,7}
    };



    public void Reshuffle()
    {
        
       r = UnityEngine.Random.Range(0, 5);
       
    }


    public void initBoardDataGOC(bool isBlack)
    {
        int val = PlayerPrefs.GetInt("switchChar" + RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw, 1);
        setSwitchPresidentPos(val == 0);

        Reshuffle();

        int[] arr = randPosGOC1.ElementAt(r);

        // User Players
        userPlayers.Add(createCharacter(new Char_Point(arr[0], 0, colour.BLACK), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[2], 0, colour.WHITE), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[1], 0, colour.BLACK), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[0], 0, colour.WHITE), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[2], 0, colour.BLACK), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[1], 0, colour.WHITE), CharacterDef.AIRFORCE, Owner.USER));


        userPlayers.Add(createCharacter(new Char_Point(arr[3], 0, colour.BOTH), CharacterDef.ARMY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[4], 0, colour.BOTH), CharacterDef.PRESIDENT, Owner.USER));
        

        userPlayers.Add(createCharacter(new Char_Point(arr[5], 0, colour.BLACK), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[7], 0, colour.WHITE), CharacterDef.AIRFORCE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[6], 0, colour.BLACK), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[5], 0, colour.WHITE), CharacterDef.NAVY, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[6], 0, colour.WHITE), CharacterDef.MARINE, Owner.USER));
        userPlayers.Add(createCharacter(new Char_Point(arr[7], 0, colour.BLACK), CharacterDef.MARINE, Owner.USER));




        for (int i = 0; i < 8; i++)
        {
            userPlayers.Add(createCharacter(new Char_Point(i, 1, colour.BLACK), CharacterDef.COASTGUARD, Owner.USER));
            userPlayers.Add(createCharacter(new Char_Point(i, 1, colour.WHITE), CharacterDef.COASTGUARD, Owner.USER));
        }


        Debug.Log("RPC Send GOC OPP:"+r);
        RoomPlayer.Local.RPC_SyncGOCOpp(r);




        this.isBlack = isBlack;
        updateArrayPosition();
        updateStatusAfterMove();
    }

    public void initBoardDataOppGOC(RoomPlayer lobbyPlayer)
    {
        int r = RoomPlayer.Local.gocPosOpp;
        Debug.Log("RPC RCVD GOC OPP:" + r);

        List<Character> players = oppPlayers;
        uint playerId = lobbyPlayer.GetComponent<NetworkObject>().Id.Raw;

        Debug.Log("player Id:"+playerId);

        if (FindObjectOfType<MultiplayerGamePlay>().isMyTeam(playerId))
        {
            players = userPlayers;
        }

        int[] arr = randPosGOC1.ElementAt(r);

        // Computer Players
        players.Add(createCharacter(new Char_Point(7 - arr[0], 7-0, colour.BLACK), CharacterDef.MARINE, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[2], 7-0, colour.WHITE), CharacterDef.MARINE, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[1], 7-0, colour.BLACK), CharacterDef.NAVY, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[0], 7-0, colour.WHITE), CharacterDef.NAVY, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[2], 7-0, colour.BLACK), CharacterDef.AIRFORCE, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[1], 7-0, colour.WHITE), CharacterDef.AIRFORCE, Owner.COMPUTER));


        players.Add(createCharacter(new Char_Point(7 - arr[3], 7-0, colour.BOTH), CharacterDef.ARMY, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[4], 7-0, colour.BOTH), CharacterDef.PRESIDENT, Owner.COMPUTER));


        players.Add(createCharacter(new Char_Point(7 - arr[5], 7-0, colour.BLACK), CharacterDef.AIRFORCE, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[7], 7-0, colour.WHITE), CharacterDef.AIRFORCE, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[6], 7-0, colour.BLACK), CharacterDef.NAVY, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[5], 7-0, colour.WHITE), CharacterDef.NAVY, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[6], 7-0, colour.WHITE), CharacterDef.MARINE, Owner.COMPUTER));
        players.Add(createCharacter(new Char_Point(7 - arr[7], 7-0, colour.BLACK), CharacterDef.MARINE, Owner.COMPUTER));

        for (int i = 0; i < 8; i++)
        {
            players.Add(createCharacter(new Char_Point(7 - i, 7 - 1, colour.BLACK), CharacterDef.COASTGUARD, Owner.COMPUTER));
            players.Add(createCharacter(new Char_Point(7 - i, 7 - 1, colour.WHITE), CharacterDef.COASTGUARD, Owner.COMPUTER));
        }



        updateArrayPosition();
        updateStatusAfterMove();

        Debug.Log("GOC OPP set with r=" + r);
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
                oppChips.Add(createCharacter(new Char_Point(i, 8, colour.WHITE), CharacterDef.POISON_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(i, 8, colour.BLACK), CharacterDef.ATOMIC, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(i, 9, colour.BOTH), CharacterDef.BIO_GAS, Owner.COMPUTER));
            }
            else
            {
                oppChips.Add(createCharacter(new Char_Point(i, 8, colour.BLACK), CharacterDef.POISON_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(i, 8, colour.WHITE), CharacterDef.ATOMIC, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(i, 9, colour.BOTH), CharacterDef.HYD_NUKE, Owner.COMPUTER));
            }
        }

        // Computer Chips in columns
        for (int i = 4; i < 8; i++)
        {
            if (i % 2 == 0)
            {
                oppChips.Add(createCharacter(new Char_Point(-1, i, colour.BLACK), CharacterDef.POISON_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(-1, i, colour.WHITE), CharacterDef.ATOMIC, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(8, i, colour.WHITE), CharacterDef.POISON_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(8, i, colour.BLACK), CharacterDef.ATOMIC, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(-2, i, colour.BOTH), CharacterDef.BIO_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(9, i, colour.BOTH), CharacterDef.HYD_NUKE, Owner.COMPUTER));
            }
            else
            {
                oppChips.Add(createCharacter(new Char_Point(-1, i, colour.WHITE), CharacterDef.POISON_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(-1, i, colour.BLACK), CharacterDef.ATOMIC, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(8, i, colour.BLACK), CharacterDef.POISON_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(8, i, colour.WHITE), CharacterDef.ATOMIC, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(9, i, colour.BOTH), CharacterDef.BIO_GAS, Owner.COMPUTER));
                oppChips.Add(createCharacter(new Char_Point(-2, i, colour.BOTH), CharacterDef.HYD_NUKE, Owner.COMPUTER));
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
            president = getPlayerByName(CharacterDef.PRESIDENT, oppPlayers);
            marine_left_1 = oppPlayers[0];
            marine_left_2 = oppPlayers[1];
            marine_right_1 = oppPlayers[12];
            marine_right_2 = oppPlayers[13];
        }

        if (!president.getIsFirstMove() /*|| isPresidentInCheck(whichOne)*/)
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
                if (!Character.CheckIfEmptyLeft(check_point, userPlayersPosition, oppPlayersPosition, projectitlePosition, ref left_point))
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
                if (!Character.CheckIfEmptyRight(check_point, userPlayersPosition, oppPlayersPosition, projectitlePosition, ref right_point))
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

                for (int i = 0; i < oppChips.Count; i++)
                {
                    if (oppChips[i].isAlive() == State.ALIVE && (oppChips[i].getPosition().x == posible_chip_point.x && oppChips[i].getPosition().y == posible_chip_point.y))
                    {
                        bool isApplicable = false;
                        if (player.getName() == CharacterDef.COASTGUARD)
                        {
                            if (oppChips[i].getName() == CharacterDef.POISON_GAS ||
                            oppChips[i].getName() == CharacterDef.ATOMIC)
                            {

                                isApplicable = true;
                            }
                        }
                        else if (player.getName() == CharacterDef.NAVY)
                        {
                            if (oppChips[i].getName() == CharacterDef.BIO_GAS ||
                            oppChips[i].getName() == CharacterDef.HYD_NUKE)
                            {

                                isApplicable = true;
                            }
                        }

                        if (isApplicable)
                        {

                            stat_change.Add(new moveStatus
                            (
                            oppChips[i],
                            oppChips[i].getPosition(),
                            oppChips[i].getIsFirstMove(),
                            State.ALIVE
                            ));

                            if (player.getOwner() == oppChips[i].getOwner())
                            {
                                // Return to initial position
                                oppChips[i].SetAlive(State.ALIVE);
                                oppChips[i].setPosition(oppChips[i].getInitPos());
                            }
                            else
                            {
                                oppChips[i].SetAlive(State.DIED);
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

            
            
            /*if (movePlayer.isChip() && landOnPlayer.isChip())
            {
                movePlayer.SetAlive(State.DIED);
                landOnPlayer.SetAlive(State.DIED);

                //if(turnStateSeqnce.Peek() == MultiplayerTurnState.OPP2_TURN)
                //{
                //    landOnPlayer.setIsOutByComp2(true);
                //}
                //else if (turnStateSeqnce.Peek() == MultiplayerTurnState.FRND_OPP_TURN)
                //{
                //    Debug.Log("FRND TURN LAND ON CHIP::" + turnStateSeqnce.Peek().ToString());
                //    landOnPlayer.setIsOutByComp3(true);
                //}

            }*/
            /*else if (movePlayer.getName() == CharacterDef.ATOMIC )
            {
                // removed from the game.
                landOnPlayer.SetAlive(State.DIED);

                movePlayer.SetAlive(State.DIED);

                *//*if (turnStateSeqnce.Peek() == MultiplayerTurnState.OPP2_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == MultiplayerTurnState.FRND_OPP_TURN)
                {
                    Debug.Log("FRND TURN LAND ON CHIP::" + turnStateSeqnce.Peek().ToString());

                    landOnPlayer.setIsOutByComp3(true);
                }*//*

            }*/
            /*else if (movePlayer.getName() == CharacterDef.HYD_NUKE)
                
            {
                movePlayer.SetAlive(State.DIED);
                landOnPlayer.SetAlive(State.DIED);

                *//*if (turnStateSeqnce.Peek() == MultiplayerTurnState.OPP2_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == MultiplayerTurnState.FRND_OPP_TURN)
                {

                    Debug.Log("FRND  TURN LAND ON CHIP::" + turnStateSeqnce.Peek().ToString());

                    landOnPlayer.setIsOutByComp3(true);
                }*//*
            }
            else if(movePlayer.getName() == CharacterDef.POISON_GAS)
            {
                movePlayer.SetAlive(State.DIED);
                landOnPlayer.SetAlive(State.CAPTURE);

                *//*if (turnStateSeqnce.Peek() == MultiplayerTurnState.OPP2_TURN)
                {
                    landOnPlayer.setIsOutByComp2(true);
                }
                else if (turnStateSeqnce.Peek() == MultiplayerTurnState.FRND_OPP_TURN)
                {
                    landOnPlayer.setIsOutByComp3(true);
                }*//*
            }*/
            //else
            //{
                // POW
                landOnPlayer.SetAlive(State.CAPTURE);

                /* if (turnStateSeqnce.Peek() == MultiplayerTurnState.OPP2_TURN)
                 {
                     landOnPlayer.setIsOutByComp2(true);
                 }
                 else if (turnStateSeqnce.Peek() == MultiplayerTurnState.FRND_OPP_TURN)
                 {

                     Debug.Log("FRND BOT TURN LAND ON CHAR::"+ turnStateSeqnce.Peek().ToString());

                     landOnPlayer.setIsOutByComp3(true);
                 }*/



                CivBasedMechanics(movePlayer, landOnPlayer);
                

            //}
            result = true;
        }

        return result;

    }

    public void CivBasedMechanics(Character movePlayer, Character landOnPlayer)
    {

        Debug.Log("Land on player name::" + landOnPlayer.getName());
        Debug.Log("Move player name::" + movePlayer.getName());

        Debug.Log("Opp Civ::" + RoomPlayer.currCivOpp);

        if (RoomPlayer.currCivOpp == 1)//vikings
        {
            if (movePlayer.getName() == CharacterDef.COASTGUARD)
            {

                movePlayer.SetAlive(State.CAPTURE);
                /*
                Debug.Log("Move Player status::" + movePlayer.isAlive());

                Debug.Log("@@updating pow and die list VIKING's++");

                

                GameObject.FindObjectOfType<MultiplayerGamePlay>().UpdatePlayerPowAndDieList(false, true);*/

                ///////////////////////////

                /*MultiplayerGamePlay multiplayerGamePlay = GameObject.FindObjectOfType<MultiplayerGamePlay>();

                indexPlayer tag = getindexByCharacter(movePlayer);
                Debug.Log("@@The tag1:"+tag.index);
                Button btn = multiplayerGamePlay.getButtonByTag(multiplayerGamePlay.GetUserCharBtns(), tag.index);
                btn.transform.parent = null;
                btn.transform.SetParent(multiplayerGamePlay.GetUserPowTransform()*//*.GetChild(0).GetComponent<RectTransform>().transform*//*);
                btn.transform.localPosition = Vector2.zero;

                btn.transform.localScale = new Vector3(0.5f,0.5f,1);
                Debug.Log("@@The tag2:" + btn.tag);*/



                //GameObject.FindObjectOfType<MultiplayerGamePlay>().UpdateOpponentPowAndDieList(false, true);


                PutPlayerInPow(movePlayer);

            }
        }
        else if (RoomPlayer.currCivOpp == (int)GameConstants.Civilisation.Haitians)
        {
            //checking which piece i am capturing when opponent is Haitian
            //if captured piece has Haitian at its adjacent sides
            //then my piece will be killed/captured as well

            Debug.Log("FOR HAITIANS::OPP IS HAITIAN!!!");


            Char_Point capturedOppPos = landOnPlayer.getPosition();

            Debug.Log(capturedOppPos.x + "::FOR HAITIANS:: CAPTURED POS!!!" + capturedOppPos.y);

            Debug.Log("FOR HAITIANS::Checking for adjacent position now!!!");

            bool isWitch = CheckAdjacentPositionsForWitch(0, capturedOppPos, oppPlayers).Count() > 0;
            if (isWitch)
            {
                Debug.Log("FOR HAITIANS::WITCH AT ADJACENT OF PIECE I CAPTURED!!!");
                Debug.Log("FOR HAITIANS::Setting status captured now!!!");


                movePlayer.SetAlive(State.CAPTURE);
                //Debug.Log("@@updating pow and die list");
                //GameObject.FindObjectOfType<MultiplayerGamePlay>().UpdatePlayerPowAndDieList(false, true);
                PutPlayerInPow(movePlayer);
            }
            else
            {
                Debug.Log("FOR HAITIANS::WITCH NOT AT ADJACENT OF PIECE I CAPTURED!!!!!!");
            }
        }
    }

    public void PutPlayerInPow(Character movePlayer)
    {
        MultiplayerGamePlay multiplayerGP = GameObject.FindObjectOfType<MultiplayerGamePlay>();

        indexPlayer tag = getindexByCharacter(movePlayer);

        Button btt = multiplayerGP.getButtonByTag(multiplayerGP.GetUserCharBtns(), tag.index);

        /*movePlayer.SetIsCaptured(true);

        int cx = 7 - btt.GetComponent<GridInfo>().x;
        int cy = 7 - btt.GetComponent<GridInfo>().y;
        int cColour = (int)btt.GetComponent<GridInfo>().colour;
        uint localId = RoomPlayer.Local.GetComponent<NetworkObject>().Id.Raw;
        RoomPlayer.Local.RPC_SyncCaptured(localId, cx, cy, (int)cColour, false);*/

        Destroy(btt.gameObject);

        GameObject powGO = Instantiate(new GameObject(), multiplayerGP.GetOppPowTransform());
        Image img = powGO.AddComponent<Image>();
        int option = (int)movePlayer.getName();
        if (option == 3)//Knight
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.NavyW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.NavyB.sprite;
            }
        }
        else if (option == 4)//Tower
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.MarineW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.MarineB.sprite;
            }
        }
        else if (option == 2)//elephant
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.AirForceW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.AirForceB.sprite;
            }
        }
        else if (option == 1)//Wazir
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.ArmyW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.ArmyB.sprite;
            }
        }
        else if (option == 9)//Horsemen
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.horseMenSprW;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.horseMenSprB;
            }
        }
        else if (option == 10)//Witch
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.WitchW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.WitchB.sprite;
            }
        }
        else if (option == 5)//pawn
        {
            if (RoomPlayer.Local.IsLeader)
            {
                img.sprite = multiplayerGP.charHandler.CoastGuardW.sprite;
            }
            else
            {
                img.sprite = multiplayerGP.charHandler.CoastGuardB.sprite;
            }
        }
    }

    public void updateCastleMoveIfany(Char_Point movePos, Character player, int whichOne, List<moveStatus> stat_change)
    {
        Char_Point castle_1 = new Char_Point(1, player.getPosition().y, colour.BOTH);
        Char_Point castle_2 = new Char_Point(6, player.getPosition().y, colour.BOTH);


        Character marine1 = null;
        Character marine2 = null;
        List<Character> yourPlayers = whichOne>0 ? userPlayers : oppPlayers;
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

        var yourPosition = coastGuard.getOwner() == Owner.USER ? userPlayersPosition : oppPlayersPosition;
        var opponentPosition = coastGuard.getOwner() == Owner.USER ? oppPlayersPosition : userPlayersPosition;
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
        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;
        if(myCiv != (int)GameConstants.Civilisation.Spanish)
        {
            return false;
        }


        Character president = whichOne>0 ? getPlayerByName(CharacterDef.PRESIDENT, userPlayers) : getPlayerByName(CharacterDef.PRESIDENT, oppPlayers);
        HashSet<int> couldBeKilled = president.getOwner() == Owner.USER ? couldBeKilledByOpp : couldBeKilledByUser;

        if (president.isAlive() != State.ALIVE)
            return false;

        if (couldBeKilled.Contains(president.getPosition().index()))
            return true;

        return false;

    }
    public MultiplayerGameState isGameOver()
    {
        Character userPresident = getPlayerByName(CharacterDef.PRESIDENT, userPlayers);
        Character oppPresident = getPlayerByName(CharacterDef.PRESIDENT, oppPlayers);

        int myCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currMyCiv;
        int oppCiv = GameObject.FindObjectOfType<MultiplayerGamePlay>().currOppCiv;

        if (myCiv != (int)GameConstants.Civilisation.French ||
            myCiv != (int)GameConstants.Civilisation.Spanish)
        {
            if (userPresident != null)
            {
                if (userPresident.isAlive() != State.ALIVE /*|| !isValidMoveAvailable(1)*/)
                    return MultiplayerGameState.OPP_WIN;
            }
        }

        if (oppCiv != (int)GameConstants.Civilisation.French ||
            oppCiv != (int)GameConstants.Civilisation.Spanish)
        {
            if (oppPresident != null)
            {
                if (oppPresident.isAlive() != State.ALIVE /*|| !isValidMoveAvailable(0)*/)
                    return MultiplayerGameState.USER_WIN;
            }
        }

        int alivePieces = 0;
        foreach (var player in userPlayers)
        {
            if (player.isAlive() == State.ALIVE)
                alivePieces++;
        }

        if (alivePieces <= 1)
            return MultiplayerGameState.OPP_WIN;

        alivePieces = 0;
        foreach (var player in oppPlayers)
        {
            if (player.isAlive() == State.ALIVE)
                alivePieces++;
        }

        if (alivePieces <= 1)
            return MultiplayerGameState.USER_WIN;

        return MultiplayerGameState.ONGOING;

    }
    /*public bool isValidMoveAvailable(int whichOne)
    {
        List<Character> players = whichOne>0 ? userPlayers : oppPlayers;
        List<Char_Point> yourPlayerPosition = whichOne>0 ? userPlayersPosition : oppPlayersPosition;
        List<Char_Point> opponentPlayerPosition = whichOne>0 ? oppPlayersPosition : userPlayersPosition;
        HashSet<int> couldBeKilled = whichOne>0 ? couldBeKilledByOpp : couldBeKilledByUser;

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

    }*/

   
}
