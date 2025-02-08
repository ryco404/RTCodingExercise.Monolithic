using System.Collections.Generic;
using NUnit.Framework;
using RTCodingExercise.Monolithic.Services;

namespace RTCodingExercise.Monolithic.Tests.Services
{
    [TestFixture]
    public class PlateLetterHelperTests
    {
        [Test]
        public void GetLettersFromPlate_ShouldConvertFirstThreeLettersOfPlate()
        {
            var helper = new PlateLetterHelper();
            var testPlate = "203LL4";
            var expectedResult = "ZOE";

            var actualResult = helper.GetLettersFromPlate(testPlate);

            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GetLettersFromPlate_ShouldConvertFirstThreeLettersOfPlateWithCustomConversions()
        {
            var helper = new PlateLetterHelper(new Dictionary<char, char> {
                ['0'] = 'A',
                ['1'] = 'B',
                ['2'] = 'C'
            });
            var testPlate = "012";
            var expectedResult = "ABC";

            var actualResult = helper.GetLettersFromPlate(testPlate);
            
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [TestCase("Cam", "CAM,C4M")]
        [TestCase("Tom", "TOM,7OM,70M,T0M")]
        [TestCase("Grace", "GRACE,6RACE,6R4CE,6R4C3,6RAC3,GR4CE,GR4C3,GRAC3")]
        public void GetAllPossiblePlatesFromSearch_ShouldReturnAllPossibleCombinationsOfSearch( 
            string search, string expectedCommaSeparated)
        {
            var helper = new PlateLetterHelper();
            var expectedSearches = expectedCommaSeparated.Split(',');

            var actualSearches = helper.GetAllPossiblePlatesFromSearch(search);

            Assert.That(actualSearches, Is.EquivalentTo(expectedSearches));
        }

        [Test]
        public void GetAllPossiblePlatesFromSearch_LimitsLengthOfSearch()
        {
            // RC: Simulate someone trying to DoS our app
            var search = new string('O', 50);
            var helper = new PlateLetterHelper();

            var actualSearches = helper.GetAllPossiblePlatesFromSearch(search);
            
            // ...But it should limit to the maximum plate length
            Assert.That(actualSearches.Count, Is.EqualTo(System.Math.Pow(2, PlateLetterHelper.MaxPlateLength)));
        }
    }
}
