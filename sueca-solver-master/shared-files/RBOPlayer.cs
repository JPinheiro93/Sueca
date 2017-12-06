using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class RBOPlayer : RPlayer
    {

        public RBOPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
                chosenCard = PIMC.Execute(_id, infoSet, 1, new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }, new List<int> { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000 });
            }

            return chosenCard;
        }

    }
}