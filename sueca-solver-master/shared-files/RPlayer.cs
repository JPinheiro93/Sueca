using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public abstract class RPlayer : ArtificialPlayer
    {
        protected InformationSet infoSet;

        public RPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId) : base(id)
        {
            infoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
        }

        override public void AddPlay(int playerID, int card)
        {
            infoSet.AddPlay(playerID, card);
        }


        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return infoSet.GetWinnerAndPointsAndTrickNumber();
        }

        public int GetCurrentTrickWinner()
        {
            return infoSet.GetCurrentTrickWinner();
        }

        public int GetCurrentTrickPoints()
        {
            return infoSet.GetCurrentTrickPoints();
        }

        public int GetZeroSumTrickScore()
        {
            return infoSet.GetZeroSumTrickScore();
        }

        public bool HasNewTrickWinner()
        {
            return infoSet.HasNewTrickWinner();
        }

        public bool HasNewTrickTeamWinner()
        {
            return infoSet.HasNewTrickTeamWinner();
        }

        public int GetTrickIncrease()
        {
            return infoSet.GetTrickIncrease();
        }
        
        public float PointsPercentage()
        {
            float alreadyMadePoints = infoSet.MyTeamPoints + infoSet.OtherTeamPoints;
            if (alreadyMadePoints == 0.0f)
            {
                return 0.5f;
            }
            return infoSet.MyTeamPoints / alreadyMadePoints;
        }

        public int GetHandSize()
        {
            return infoSet.GetHandSize();
        }

        public string GetLastPlayInfo()
        {
            return infoSet.GetLastPlayInfo();
        }

        public bool IsLastPlayOfTrick()
        {
            return infoSet.IsLastPlayOfTrick();
        }


        public int GetNextPlayerId()
        {
            return infoSet.GetNextPlayerId();
        }


        //attribute the event to the winner when he is from my team and blame himself or the partner when winner is an opponent
        public int GetResposibleForLastTrick()
        {
            return infoSet.GetCurrentTrickResponsible();
        }
    }
}