using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class RKOPlayer : RPlayer
    {

        public RKOPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id, initialHand, trumpCard, trumpPlayerId)
        {
        }

        override public int Play()
        {
            int chosenCard;

            if (infoSet.GetHandSize() > 10)
            {
                chosenCard = infoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = PINK.Execute(_id, infoSet, 1, new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }, new List<int> { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000 });
            }

            return chosenCard;
        }

    }
}