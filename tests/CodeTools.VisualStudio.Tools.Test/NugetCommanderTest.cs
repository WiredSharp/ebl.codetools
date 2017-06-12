#region File
// ///
// ///
// /// Created by: Eric BRUNEL
// /// on 2017-03-14 (11:53)
// ///
// ///
#endregion


using NUnit.Framework;

namespace CodeTools.VisualStudio.Tools.Test
{
    [TestFixture]
    public class NugetCommanderTest
    {
        [Test]
        public void i_can_instantiate()
        {
            NugetCommander.Main(new[] { "" });
        }
    }
}