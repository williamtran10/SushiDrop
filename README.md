# SushiDrop

By: William Tran and Adrian Cerejo
Made in C# using Visual Studio 2017 without the use of Unity.

Summary
-----------
A sushi-themed match-3 game similar to Candy Crush or Bejeweled.

Features:
-----------
- Checking for valid tile clicks
- Checking for matches of a valid length in either direction
- Two time limits: 3 minutes and 10 minutes
- Animations for: swapping tiles, disappearing tiles, and dropping columns, which scales to the length that each column needs to drop
- Hint system that can show a valid match and will end the game if there are no possible moves left
- Points system that rewards combos and larger matches
- Combo system that increases the points mulitplier and checks for matches using the tiles that just dropped
- Highscores stored in a .txt for top three scores of each time limit which can be cleared

To-do:
-----------
- Add a hardcoded test case with the potential for a very high combo for showcasing purposes
- Generalize code for more customizability (ex. add more rows, columns, change size)
