using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuecaSolver
{
    public static class PIMC
    {
        // version = 0 - The search is the MinMax algorithm
        // version = 1 - The search strats at the last 5 tricks with the MinMax algorithm, until there choices are rulebased
        public static int Execute(int playerId, InformationSet infoSet, int version, List<int> numIterations = null, List<int> depthLimits = null)
        {
            List<int> possibleMoves = infoSet.GetPossibleMoves();
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (int card in possibleMoves)
            {
                dict.Add(card, 0);
            }


            int N, depthLimit, handSize = infoSet.GetHandSize();
            if (numIterations != null)
            {
                N = numIterations[handSize - 1];
                if (depthLimits != null)
                {
                    depthLimit = depthLimits[handSize - 1];
                }
                else
                {
                    depthLimit = 10;
                }
            }
            else
            {
                N = 50;
                depthLimit = 1;
            }

            var watch = Stopwatch.StartNew();
            for (int i = 0; i < N; i++)
            {
                if (watch.ElapsedMilliseconds > Sueca.MAX_MILISEC_DELIBERATION)
                {
                    watch.Stop();
                    return infoSet.RuleBasedDecision();
                }

                List<List<int>> playersHands = infoSet.Sample();

                PerfectInformationGame game;
                int cardUtility;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    if (version == 0)
                    {
                        MaxNode p0 = new MaxNode(playerId, playersHands[0], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        MinNode p1 = new MinNode((playerId + 1) % 4, playersHands[1], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        MaxNode p2 = new MaxNode((playerId + 2) % 4, playersHands[2], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        MinNode p3 = new MinNode((playerId + 3) % 4, playersHands[3], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        game = new PerfectInformationGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetPastMoves(), infoSet.MyTeamPoints, infoSet.OtherTeamPoints);
                    }
                    else if(version == 1)
                    {
                        MaxNode p0 = new MaxNode(playerId, playersHands[0], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        RuleBasedNode p1 = new RuleBasedNode((playerId + 1) % 4, playersHands[1], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        RuleBasedNode p2 = new RuleBasedNode((playerId + 2) % 4, playersHands[2], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        RuleBasedNode p3 = new RuleBasedNode((playerId + 3) % 4, playersHands[3], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        game = new PerfectInformationGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetPastMoves(), infoSet.MyTeamPoints, infoSet.OtherTeamPoints);
                    }
                    else
                    {
                        Console.WriteLine("PIMC::Execute >> Undefinied version of the algorithm.");
                        MaxNode p0 = new MaxNode(playerId, playersHands[0], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        MinNode p1 = new MinNode((playerId + 1) % 4, playersHands[1], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        MaxNode p2 = new MaxNode((playerId + 2) % 4, playersHands[2], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        MinNode p3 = new MinNode((playerId + 3) % 4, playersHands[3], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        game = new PerfectInformationGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetPastMoves(), infoSet.MyTeamPoints, infoSet.OtherTeamPoints);
                    }

                    cardUtility = game.SampleGame(depthLimit, card);
                    if (cardUtility > 120 || cardUtility < -120)
                    {
                        Console.WriteLine("lol");
                    }
                    dict[card] += cardUtility; 
                }
            }
            watch.Stop();


            int bestCard = -1;
            int bestValue = Int32.MinValue;

            int suitseven = 4, suitace = 5;
            int valueseven = Int32.MinValue;
            int valueace = valueseven + 1;
            int key = -1;
            Console.WriteLine("Printing dictionary:");

            foreach (KeyValuePair<int, int> card in dict)
            {
                if (Card.GetRank(card.Key) == 8)
                {
                    suitseven = Card.GetSuit(card.Key);
                    valueseven = card.Value;
                }

                if (Card.GetRank(card.Key) == 9)
                {
                    suitace = Card.GetSuit(card.Key);
                    valueace = card.Value;
                }

                if (suitseven == suitace)
                {
                    if (valueseven > valueace)
                    {
                        Console.Write("-----" + card.Key + "----");
                        //suitace *= 10;
                        //int key = 9 + suitace;
                        // var newEntry = new KeyValuePair<int, int>(card.Key, valueseven);
                        key = card.Key;
                        
                    }
                }
                Console.Write(Card.ToString(card.Key) + ":" + card.Value + ", ");

            }
            if(key != -1)
            {
                dict.Remove(key);
                dict.Add(key, valueseven);
            }

            Console.WriteLine();

            foreach (KeyValuePair<int, int> cardValue in dict)
            {
                
                if (cardValue.Value >= bestValue){
                    bool flag = false;

                    if (Card.ToString(cardValue.Key)[0] == '7')
                    {
                        foreach (KeyValuePair<int, int> card in dict)
                        {
                            if (Card.ToString(card.Key)[0] == 'A' && Card.ToString(card.Key)[1] == Card.ToString(cardValue.Key)[1])
                            {
                                flag = true;
                                //Console.WriteLine("Switch 7 with A");
                            }
                        }
                    }

                    if (!flag)
                    {
                            bestValue = (int)cardValue.Value;
                            bestCard = cardValue.Key;
                            Console.WriteLine("bestCard: "+Card.ToString(bestCard));
                    }
                    else
                    {
                        Console.WriteLine("Switch 7 with A");

                    }

                }

            }
            Console.WriteLine();

            if (bestCard == -1)
            {
                Console.WriteLine("Trouble at InformationSet.GetBestCardAndValue()");
            }

            return bestCard;
        }


        public static int ExecuteWithHybridSearch(int playerId, InformationSet infoSet, List<int> numDistributions, List<int> numIterationsPerCard)
        {
            List<int> possibleMoves = infoSet.GetPossibleMoves();
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (int card in possibleMoves)
            {
                dict.Add(card, 0);
            }

            int N, M, handSize = infoSet.GetHandSize();
            N = numDistributions[handSize - 1];
            M = numIterationsPerCard[handSize - 1];
            Object thisLock = new Object();

            Parallel.For(0, N,
                    new ParallelOptions { MaxDegreeOfParallelism = Sueca.HYBRID_NUM_THREADS },
                    () => new int[possibleMoves.Count * 2], //{card, value, card, value, ...}

                    (int i, ParallelLoopState state, int[] cardsSamplingValues) =>
                    {
                        return HybridSearchMainLoop(cardsSamplingValues, thisLock, playerId, infoSet, possibleMoves, M, handSize);
                    },

                    (int[] cardsSamplingValues) =>
                    {
                        for (int i = 0; i < cardsSamplingValues.Length / 2; i++)
                        {
                            dict[cardsSamplingValues[i * 2]] += cardsSamplingValues[i * 2 + 1];
                        }
                    });

            int bestCard = -1;
            int bestValue = Int32.MinValue;

            foreach (KeyValuePair<int, int> cardValue in dict)
            {
                if (cardValue.Value > bestValue)
                {
                    bestValue = (int)cardValue.Value;
                    bestCard = cardValue.Key;
                }
            }

            if (bestCard == -1)
            {
                Console.WriteLine("Trouble at InformationSet.GetBestCardAndValue()");
            }

            return bestCard;
        }

        private static int[] HybridSearchMainLoop(int[] cardsSamplingValues, Object thisLock, int playerId, InformationSet infoSet, List<int> possibleMoves, int M, int handSize)
        {
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                cardsSamplingValues[i * 2] = possibleMoves[i];
                cardsSamplingValues[(i * 2) + 1] = 0;
            }

            List<List<int>> playersHands;
            lock (thisLock)
            {
                playersHands = infoSet.Sample();
            }
            PerfectInformationGame game;
            int cardUtility;
            for (int j = 0; j < possibleMoves.Count; j++)
            {
                int card = possibleMoves[j];
                int hybridTrickChange = handSize - 5;

                if (handSize > 5)
                {
                    for (int k = 0; k < M; k++)
                    {
                        RuleBasedNode p0 = new RuleBasedNode(playerId, playersHands[0], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        RuleBasedNode p1 = new RuleBasedNode((playerId + 1) % 4, playersHands[1], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        RuleBasedNode p2 = new RuleBasedNode((playerId + 2) % 4, playersHands[2], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        RuleBasedNode p3 = new RuleBasedNode((playerId + 3) % 4, playersHands[3], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                        game = new PerfectInformationGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetPastMoves(), infoSet.MyTeamPoints, infoSet.OtherTeamPoints, true, hybridTrickChange);
                        cardUtility = game.SampleGame(1000, card);
                        cardsSamplingValues[j * 2 + 1] += cardUtility;
                    }
                }
                else
                {
                    MaxNode p0 = new MaxNode(playerId, playersHands[0], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    MinNode p1 = new MinNode((playerId + 1) % 4, playersHands[1], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    MaxNode p2 = new MaxNode((playerId + 2) % 4, playersHands[2], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    MinNode p3 = new MinNode((playerId + 3) % 4, playersHands[3], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    game = new PerfectInformationGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetPastMoves(), infoSet.MyTeamPoints, infoSet.OtherTeamPoints);
                    cardUtility = game.SampleGame(1000, card);
                    cardsSamplingValues[j * 2 + 1] += cardUtility;
                }
            }
            return cardsSamplingValues;
        }


        public static int ExecuteWithTimeLimit(int playerId, InformationSet infoSet, List<int> depthLimits)
        {
            List<int> possibleMoves = infoSet.GetPossibleMoves();
            if (possibleMoves.Count == 1)
            {
                return possibleMoves[0];
            }

            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (int card in possibleMoves)
            {
                dict.Add(card, 0);
            }

            int depthLimit, handSize = infoSet.GetHandSize();
            int n = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long time = sw.ElapsedMilliseconds;
            depthLimit = depthLimits[handSize - 1];
            for (; time < 2000; )
            {
                n++;
                List<List<int>> playersHands = infoSet.Sample();

                PerfectInformationGame game;
                int cardUtility;

                for (int j = 0; j < possibleMoves.Count; j++)
                {
                    int card = possibleMoves[j];
                    MaxNode p0 = new MaxNode(playerId, playersHands[0], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    MinNode p1 = new MinNode((playerId + 1) % 4, playersHands[1], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    MaxNode p2 = new MaxNode((playerId + 2) % 4, playersHands[2], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    MinNode p3 = new MinNode((playerId + 3) % 4, playersHands[3], infoSet.TrumpCard, infoSet.TrumpPlayerId);
                    game = new PerfectInformationGame(p0, p1, p2, p3, infoSet.Trump, infoSet.GetPastMoves(), infoSet.MyTeamPoints, infoSet.OtherTeamPoints);
                    cardUtility = game.SampleGame(depthLimit, card);
                    dict[card] += cardUtility; 
                }
                time = sw.ElapsedMilliseconds;
            }

            sw.Stop();

            int bestCard = -1;
            int bestValue = Int32.MinValue;

            foreach (KeyValuePair<int, int> cardValue in dict)
            {
                if (cardValue.Value >= bestValue)
                {
                    bestValue = (int)cardValue.Value;
                    bestCard = cardValue.Key;
                }
            }

            if (bestCard == -1)
            {
                Console.WriteLine("Trouble at InformationSet.GetBestCardAndValue()");
            }

            return bestCard;
        }
    }
}