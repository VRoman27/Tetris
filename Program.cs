using System;
using System.Threading;

int[,] map = CreateMap();
int spawnX = 0;
int spawnY = 5;
int currentX = spawnX;
int currentY = spawnY;
int currentFigure = 0;
int currentAngle = 0;
Random figureType = new Random();

int frame = 10;
int delay = 5;
int tick = 0;

int score = 0;


ConsoleKeyInfo key;
Game();
void Game()
{
    bool exit = false;
    while (!exit)
    {
        ShowMap(map, score);
        if (Console.KeyAvailable)
        {
            key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape) return;
            if (key.Key == ConsoleKey.RightArrow)
            {
                if (CollisionWall(map, CreateFigure(currentFigure, currentAngle), currentX, currentY, true))
                {
                    DeleteFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);
                    currentY++;
                    SpawnFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);
                }

            }
            if (key.Key == ConsoleKey.LeftArrow)
            {
                if (CollisionWall(map, CreateFigure(currentFigure, currentAngle), currentX, currentY, false))
                {
                    DeleteFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);
                    currentY--;
                    SpawnFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);

                }
            }
            if (key.Key == ConsoleKey.UpArrow)
            {
                DeleteFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);
                if (currentAngle < 3) currentAngle++;
                else currentAngle = 0;
                SpawnFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);
            }
            if (key.Key == ConsoleKey.DownArrow)
            {
                Gravity();
            }

        }

        if (currentFigure == 0) currentFigure = figureType.Next(1, 8);

        if (tick <= delay) tick++;
        else
        {
            Gravity();

            tick = 0;
        }

        Thread.Sleep(frame);
    }

    void Gravity()
    {

        if (ItIsFall(map, CreateFigure(currentFigure, currentAngle), currentX, currentY))
        {
            if (currentX != spawnX)
            {
                DeleteFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);
            }
            currentX++;
            SpawnFigure(map, CreateFigure(currentFigure, currentAngle), currentX, currentY);
        }
        else
        {
            ShowMap(map, score);
            score += ClearLine(map);
            currentFigure = 0;
            currentX = spawnX;
            currentY = spawnY;
        }
    }
}

int[,] CreateFigure(int type, int angle)
{
    int[,] squareF = { { 1, 1 }, { 1, 1 } };
    int[,] lineF = { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } };
    int[,] rightLF = { { 0, 0, 1 }, { 1, 1, 1 }, { 0, 0, 0 } };
    int[,] leftLF = { { 1, 0, 0 }, { 1, 1, 1 }, { 0, 0, 0 } };
    int[,] TF = { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
    int[,] rightZF = { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
    int[,] leftZF = { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };

    return RotateFigure(SelectFigure(type), angle);

    int[,] RotateFigure(int[,] figure, int angle)
    {
        int[,] figureRotate = (int[,])figure.Clone();
        switch (angle)
        {
            case 0:
                for (int i = 0; i < figureRotate.GetLength(0); i++)
                {
                    for (int j = 0; j < figureRotate.GetLength(1); j++)
                    {
                        figureRotate[i, j] = figure[i, j];
                    }
                }
                break;
            case 1:
                for (int i = 0; i < figureRotate.GetLength(1); i++)
                {
                    for (int j = 0; j < figureRotate.GetLength(0); j++)
                    {
                        figureRotate[i, j] = figure[j, figure.GetLength(1) - 1 - i];
                    }
                }
                break;
            case 2:
                for (int i = 0; i < figureRotate.GetLength(0); i++)
                {
                    for (int j = 0; j < figureRotate.GetLength(1); j++)
                    {
                        figureRotate[i, j] = figure[figure.GetLength(0) - 1 - i, figure.GetLength(0) - 1 - j];
                    }
                }
                break;
            case 3:
                for (int i = 0; i < figureRotate.GetLength(1); i++)
                {
                    for (int j = 0; j < figureRotate.GetLength(0); j++)
                    {
                        figureRotate[i, j] = figure[figure.GetLength(0) - 1 - j, i];
                    }
                }
                break;

            default: break;
        }
        return figureRotate;
    }

    int[,] SelectFigure(int type)
    {
        switch (type)
        {
            case 1:
                return squareF;
            case 2:
                return lineF;
            case 3:
                return rightLF;
            case 4:
                return leftLF;
            case 5:
                return TF;
            case 6:
                return rightZF;
            case 7:
                return leftZF;
            default: return squareF; ;
        }
    }

}

bool ItIsFall(int[,] map, int[,] figure, int x, int y)
{
    x -= (figure.GetLength(0) - 1) / 2;
    y -= (figure.GetLength(1) - 1) / 2;
    for (int j = 0; j < figure.GetLength(1); j++)
    {
        for (int i = figure.GetLength(0) - 1; i >= 0;)
        {
            if (figure[i, j] == 1)
            {
                if (map[i + x + 1, j + y] == 1) { return false; }
                else { i = -1; }
            }
            else i--;
        }
    }
    return true;
}

bool CollisionWall(int[,] map, int[,] figure, int x, int y, bool itsRight)
{
    x -= (figure.GetLength(0) - 1) / 2;
    y -= (figure.GetLength(1) - 1) / 2;

    for (int i = 0; i < figure.GetLength(0); i++)
    {
        if (itsRight)
        {
            for (int j = figure.GetLength(1) - 1; j >= 0;)
            {
                if (figure[i, j] == 1)
                {
                    if (map[i + x, j + y + 1] == 1) { return false; }
                    else { j = -1; }
                }
                else j--;
            }
        }
        else
        {
            for (int j = 0; j < figure.GetLength(1);)
            {
                if (figure[i, j] == 1)
                {
                    if (map[i + x, j + y - 1] == 1) { return false; }
                    else { j = figure.GetLength(1); }
                }
                else j++;
            }
        }
    }
    return true;
}

int ClearLine(int[,] map)
{
    int count = 0;
    for (int i = map.GetLength(0) - 2; i > 1; i--)
    {
        int line = 0;
        for (int j = 1; j < map.GetLength(1) - 1; j++)
        {
            line += map[i, j];
        }
        if (line == map.GetLength(1) - 2)
        {
            count++;
            ShiftDown(i);
            i++;
        }
    }
    if(count == 1) return 100;
    if(count == 2) return 200;
    if(count == 3) return 300;
    if(count == 4) return 1500;
    else return 0;

    void ShiftDown(int deletedLine)
    {
        for (int i = deletedLine; i > 2; i--)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j++)
            {
                map[i, j] = map[i - 1, j];
            }
        }
    }
}

void SpawnFigure(int[,] map, int[,] figure, int x, int y)
{
    x -= (figure.GetLength(0) - 1) / 2;
    y -= (figure.GetLength(1) - 1) / 2;
    for (int i = 0; i < figure.GetLength(0); i++)
    {
        for (int j = 0; j < figure.GetLength(1); j++)
        {
            if (figure[i, j] == 1) map[i + x, j + y] = 1;
        }
    }
}

void DeleteFigure(int[,] map, int[,] figure, int x, int y)
{
    x -= (figure.GetLength(0) - 1) / 2;
    y -= (figure.GetLength(1) - 1) / 2;
    for (int i = 0; i < figure.GetLength(0); i++)
    {
        for (int j = 0; j < figure.GetLength(1); j++)
        {
            if (figure[i, j] == 1) map[i + x, j + y] = 0;
        }
    }
}

void ShowMap(int[,] array, int score)
{
    Console.Clear();
    for (var i = 0; i < array.GetLength(0); i++)
    {
        for (var j = 0; j < array.GetLength(1); j++)
        {
            if (array[i, j] == 1) Console.Write("# ");
            else Console.Write("  ");
        }
        Console.WriteLine();
    }
    Console.WriteLine($"Score:{score}");
    Console.WriteLine("Esc: выход    ←↑↓→: управление");

}

int[,] CreateMap()
{
    int[,] map = new int[22, 12];
    for (int i = 0; i < map.GetLength(0); i++)
    {
        for (int j = 0; j < map.GetLength(1); j++)
        {
            if ((i == 0 && j == 0) || (i == 0 && j == map.GetLength(1) - 1) || i == map.GetLength(0) - 1 || j == 0 || j == map.GetLength(1) - 1) map[i, j] = 1;
            else map[i, j] = 0;
        }
    }

    return map;
}
