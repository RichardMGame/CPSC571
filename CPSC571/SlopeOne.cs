/*
Original Code can be found here: http://www.cnblogs.com/kuber/articles/SlopeOne_CSharp.html

Author is Kuberski

Modified to fits purposes of the assignment
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPSC571
{
    public class Rating
    {
        public float Value { get; set; }
        public int Freq { get; set; }

        public float AverageValue
        {
            get { return Value / Freq; }
        }
    }

    public class RatingDifferenceCollection : Dictionary<string, Rating>
    {
        private string GetKey(int Item1Id, int Item2Id)
        {
            return (Item1Id < Item2Id) ? Item1Id + "/" + Item2Id : Item2Id + "/" + Item1Id;
        }

        public bool Contains(int Item1Id, int Item2Id)
        {
            return this.Keys.Contains<string>(GetKey(Item1Id, Item2Id));
        }

        public Rating this[int Item1Id, int Item2Id]
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
        public HashSet<int> _Items = new HashSet<int>();  // Tracking how many items totally

        public void AddUserRatings(IDictionary<int, float> userRatings)
        {
            foreach (var item1 in userRatings)
            {
                int item1Id = item1.Key;
                float item1Rating = item1.Value;
                _Items.Add(item1.Key);

                foreach (var item2 in userRatings)
                {
                    if (item2.Key <= item1Id) continue; // Eliminate redundancy
                    int item2Id = item2.Key;
                    float item2Rating = item2.Value;

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
        public void AddUerRatings(IList<IDictionary<int, float>> Ratings)
        {
            foreach (var userRatings in Ratings)
            {
                AddUserRatings(userRatings);
            }
        }

        public IDictionary<int, float> Predict(IDictionary<int, float> userRatings)
        {
            Dictionary<int, float> Predictions = new Dictionary<int, float>();
            foreach (var itemId in this._Items)
            {
                if (userRatings.Keys.Contains(itemId)) continue; // User has rated this item, just skip it

                Rating itemRating = new Rating();

                foreach (var userRating in userRatings)
                {
                    if (userRating.Key == itemId) continue;
                    int inputItemId = userRating.Key;
                    if (_DiffMarix.Contains(itemId, inputItemId))
                    {
                        Rating diff = _DiffMarix[itemId, inputItemId];
                        itemRating.Value += diff.Freq * (userRating.Value + diff.AverageValue * ((itemId < inputItemId) ? 1 : -1));
                        itemRating.Freq += diff.Freq;
                    }
                }
                Predictions.Add(itemId, itemRating.AverageValue);
            }
            return Predictions;
        }
    }
}
