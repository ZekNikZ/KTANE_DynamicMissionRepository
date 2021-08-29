using System;

using DMG.Parser;

namespace DMGTests {
    class MainClass {
        public static void Main(string[] args) {
            Console.WriteLine(new DMGParser(true).Parse(TestMissions.SMALL_TEST));
        }
    }
}
