using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPSC571
{
    public class Rating
    {
        public int Value { get; set; }
        public int Freq { get; set; }

        public int AverageValue
        {
            get {
                if (Freq != 0)
                {
                    return Value / Freq;
                }
                else
                    return 0;
            }
        }
    }

    public class RatingDifferenceCollection : Dictionary<string, Rating>
    {
        private string GetKey(long Item1Id, long Item2Id)
        {
            return (Item1Id < Item2Id) ? Item1Id + "/" + Item2Id : Item2Id + "/" + Item1Id;
        }

        public bool Contains(long Item1Id, long Item2Id)
        {
            return this.Keys.Contains<string>(GetKey(Item1Id, Item2Id));
        }

        public Rating this[long Item1Id, long Item2Id]
        {
            get
            {
                return this[this.GetKey(Item1Id, Item2Id)];
            }
            set { this[this.GetKey(Item1Id, Item2Id)] = value; }
        }
    }

    public class SlopeOne
    {
        public RatingDifferenceCollection _DiffMarix = new RatingDifferenceCollection();  // The dictionary to keep the diff matrix
        public HashSet<long> _Items = new HashSet<long>();  // Tracking how many items totally

        public void AddUserRatings(IDictionary<long, int> userRatings)
        {
            foreach (var item1 in userRatings)
            {
                long item1Id = item1.Key;
                int item1Rating = item1.Value;
                _Items.Add(item1.Key);

                foreach (var item2 in userRatings)
                {
                    if (item2.Key <= item1Id) continue; // Eliminate redundancy
                    long item2Id = item2.Key;
                    int item2Rating = item2.Value;

                    Rating ratingDiff;
                    if (_DiffMarix.Contains(item1Id, item2Id))
                    {
                        ratingDiff = _DiffMarix[item1Id, item2Id];
                    }
                    else
                    {
                        ratingDiff = new Rating();
                        _DiffMarix[item1Id, item2Id] = ratingDiff;
                    }

                    ratingDiff.Value += item1Rating - item2Rating;
                    ratingDiff.Freq += 1;
                }
            }
        }

        // Input ratings of all users
        public void AddUerRatings(IList<IDictionary<long, int>> Ratings)
        {
            foreach (var userRatings in Ratings)
            {
                AddUserRatings(userRatings);
            }
        }

        public IDictionary<long, int> Predict(IDictionary<long, int> userRatings)
        {
            Dictionary<long, int> Predictions = new Dictionary<long, int>();
            foreach (var itemId in this._Items)
            {
                if (userRatings.Keys.Contains(itemId)) continue; // User has rated this item, just skip it

                Rating itemRating = new Rating();

                foreach (var userRating in userRatings)
                {
                    if (userRating.Key == itemId) continue;
                    long inputItemId = userRating.Key;
                    if (_DiffMarix.Contains(itemId, inputItemId))
                    {
                        Rating diff = _DiffMarix[itemId, inputItemId];
                        itemRating.Value += diff.Freq * (userRating.Value + diff.AverageValue * ((itemId < inputItemId) ? 1 : -1));
                        itemRating.Freq += diff.Freq;
                    }
                }
                Predictions.Add(itemId, (int)itemRating.AverageValue);
            }
            return Predictions;
        }
    }
}