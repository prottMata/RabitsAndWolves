using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace RabsAndWolfs
{
    class Game
    {
        static readonly int x = 80;
        static readonly int y = 30;

        static Walls walls;
        //static Rabbit rab;
        static List<Rabbit> rabbits;
        static List<Wolf> wolves;
        //static Wolf wolf;
        static GrassFactory grassFactory;
        static Timer time;
        public static void WriteLine(string message)
        {
            return;
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(String.Format("{0,-23} {1}", DateTime.Now.ToString() + ":", message));
            }
        }

        static void Main()
        {
            rabbits = new List<Rabbit>();
            wolves = new List<Wolf>();
            Console.SetWindowSize(x + 1, y + 1);
            Console.SetBufferSize(x + 1, y + 1);
            Console.CursorVisible = false;

            walls = new Walls(x, y, '#');
            //rabbit = new Rabbit(x / 2, y / 2);
            /*for (int i = 0; i < 2; i++) {
                //Rabbit rabbit = new ;
                rabbits.Add(new Rabbit(i  10, i * 10));
             };
            */
            rabbits.Add(new Rabbit(2, 2));
            rabbits.Add(new Rabbit(7, 7));
            rabbits.Add(new Rabbit(10, 10));
            grassFactory = new GrassFactory(x, y, '$');
            wolves.Add(new Wolf(40, 5));
            wolves.Add(new Wolf(60, 20));
            //wolf = new Wolf(60, 17);



            grassFactory.CreateGrass();
            foreach (Rabbit rabbit in rabbits)
            {
                rabbit.MoveToGrass(grassFactory.grass);
                foreach(Wolf wolf in wolves)
                    wolf.MoveToRabbit(rabbit);
            }





            time = new Timer(Loop, null, 0, 200);
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    //rabbit.Rotation(key.Key);
                }
            }

        }// Main()

        static void Loop(object obj)
        {
            // foreach (Point gr in grassFactory.grass)
            //{
            //List<Rabbit> remrab =new List<Rabbit>();
            bool newGrass = false;
            foreach (Rabbit rabbit in rabbits.ToList())
            {
                for (int i = 0; i < 2; i++)
                {
                    if (walls.IsHit(rabbit.GetPresent()) || rabbit.IsHit(rabbit.GetPresent()))
                    {
                        time.Change(0, Timeout.Infinite);
                    }
                    else if ((rabbit.hungerLevel == 0) || (rabbit.lifeTime == 0))
                    {
                        rabbits.Remove(rabbit);
                        rabbit.GetPresent().Clear();
                    }
                    else if (rabbit.Eat(grassFactory.grass))
                    {
                        grassFactory.CreateGrass();
                        newGrass = true;
                        rabbit.MoveToGrass(grassFactory.grass);
                        if (rabbit.NewRab == false)
                            rabbit.NewRabbit(rabbits);


                    }
                    else if (rabbit.NewRab)
                    {
                        if (rabbit.MoveToRabbit())
                            rabbits.Add(new Rabbit(rabbit.rabbit.First().x, rabbit.rabbit.First().y));
                        rabbit.Move();
                        /*
                        if (rabbit.Avoid(rabbits))
                        {
                            if (newRabbit)
                            {
                                rabbits.Add(new Rabbit(rabbit.GetPresent().x + 2, rabbit.GetPresent().y + 2));
                                newRabbit = false;
                            }
                            rabbit.Evade();
                        }
                    }
                    else if (rabbit.Avoid(rabbits))
                    {
                        if (newRabbit)
                        {
                            rabbits.Add(new Rabbit(rabbit.GetPresent().x + 2, rabbit.GetPresent().y + 2));
                            newRabbit = false;
                        }
                        rabbit.Evade();
                        */
                    }
                    /*else if (rabbit.Avoid(wolf))
                    {
                        rabbit.EvadeWolf();
                    }*/
                    else
                    {
                        rabbit.Move();

                    }
                }
                foreach(Wolf wolf in wolves)
                    wolf.MoveToRabbit(rabbit);
                
            }
            //if (rabbits.Count > 0)
            //{
            foreach(Wolf wolf in wolves)
            {
                wolf.Move(grassFactory.grass);
                if ((rabbits.Count > 0) && (wolf.Eat(rabbits)))//(rabbit))
                {
                    WriteLine("Волк ЕСТ");
                    //rabbits.Remove(wolf.rabbitforEat);
                    WriteLine("Зайца");
                }
            }
            if (newGrass)
                foreach (Rabbit rabbit in rabbits)
                    rabbit.MoveToGrass(grassFactory.grass);
            //}
            //}

        }// Loop()
    }// class Game

    struct Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public char ch { get; set; }

        public static implicit operator Point((int, int, char) value) =>
              new Point { x = value.Item1, y = value.Item2, ch = value.Item3 };

        public static bool operator ==(Point a, Point b) =>
                (a.x == b.x && a.y == b.y) ? true : false;
        public static bool operator !=(Point a, Point b) =>
                (a.x != b.x || a.y != b.y) ? true : false;

        public void Draw()
        {
            DrawPoint(ch);
        }
        public void Clear()
        {
            DrawPoint(' ');
        }

        private void DrawPoint(char _ch)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(_ch);
        }
    }

    class Walls
    {
        private char ch;
        private List<Point> wall = new List<Point>();

        public Walls(int x, int y, char ch)
        {
            this.ch = ch;

            DrawHorizontal(x, 0);
            DrawHorizontal(x, y);
            DrawVertical(0, y);
            DrawVertical(x, y);
        }

        private void DrawHorizontal(int x, int y)
        {
            for (int i = 0; i < x; i++)
            {
                Point p = (i, y, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        private void DrawVertical(int x, int y)
        {
            for (int i = 0; i < y; i++)
            {
                Point p = (x, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        public bool IsHit(Point p)
        {
            foreach (var w in wall)
            {
                if (p == w)
                {
                    return true;
                }
            }
            return false;
        }
    }// class Walls

    enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    class Rabbit
    {
        public List<Point> rabbit;

        private Direction direction;
        private int h = 1;
        private Point previous;
        private Point present;
        public Point pointForRabbit;
        public bool NewRab;
        private int xgr, ygr;
        public int lifeTime;
        public int hungerLevel;

        bool rotate = true;

        public Rabbit(int x, int y)
        {
            direction = Direction.RIGHT;
            rabbit = new List<Point>();
            lifeTime = 50;
            hungerLevel = 30;
            NewRab = false;
            Point p = (x, y, '*');
            rabbit.Add(p);

            p.Draw();
        }



        public Point GetPresent() => rabbit.Last();

        public void NewRabbit(List<Rabbit> rabbits)
        {
            foreach(Rabbit rabbit in rabbits)
            {
                if (rabbit.hungerLevel > 7)
                {
                    pointForRabbit = rabbit.rabbit.First();
                    NewRab = true;
                }
            }
            //return false;
         
        }
        public bool MoveToRabbit()
        {
            Point p = GetPresent();

            if ((p.x - pointForRabbit.x) < 0)
            {
                direction = Direction.RIGHT;
            }
            else if ((p.x - pointForRabbit.x) > 0)
                direction = Direction.LEFT;
            if ((p.y - pointForRabbit.y) < 0)
            {
                direction = Direction.DOWN;
            }
            else if ((p.y - pointForRabbit.y) > 0)
                direction = Direction.UP;


            switch (direction)
            {
                case Direction.LEFT:
                    p.x -= h;
                    break;
                case Direction.RIGHT:
                    p.x += h;
                    break;
                case Direction.UP:
                    p.y -= h;
                    break;
                case Direction.DOWN:
                    p.y += h;
                    break;
            }
            if (p == pointForRabbit)
            {
                NewRab = false;
                return true;
            }
            return false;
        }
        public void Move()
        {
            present = GetNextPoint();
            rabbit.Add(present);

            previous = rabbit.First();
            rabbit.Remove(previous);

            previous.Clear();
            present.Draw();

            rotate = true;
            //hungerLevel--;
            //lifeTime--;
        }
        public void MoveToGrass(List<Point> grass)
        {
            Point p = GetPresent();
            int min = 1000;
            foreach (Point gr in grass)
            {
                if ((Math.Abs(gr.x - p.x) + Math.Abs(gr.y - p.y)) < min)
                {
                    min = Math.Abs(gr.x - p.x) + Math.Abs(gr.y - p.y);
                    xgr = gr.x;
                    ygr = gr.y;
                }
            }

        }
        public bool Avoid(List<Rabbit> rabbits)
        {
            Point p, pa, pr;
            pr = GetPresent();
            p = GetNextPoint();
            foreach (Rabbit rabbit in rabbits)
            {
                if (pr != rabbit.GetPresent())
                {
                    pa = rabbit.GetPresent();
                    if (p == pa)
                    {
                        return true;
                    }
                }

            }

            return false;
        }
        public bool Avoid(Wolf wolf)
        {
            Point p, pw;
            p = GetNextPoint();
            pw = wolf.GetNextPoint();
            if (p == pw)
            {
                return true;
            }

            return false;
        }

        public void EvadeWolf()
        {
            Point p = GetPresent();
            //движение по часовой
            switch (direction)
            {
                case Direction.LEFT: //если влево, то вправо 
                    p.x += h;
                    break;
                case Direction.RIGHT: //если вправо, то влево
                    p.x -= h;
                    break;
                case Direction.UP: //если вверх, то вниз
                    p.y += h;
                    break;
                case Direction.DOWN:  //если вниз, то вверх
                    p.y -= h;
                    break;
            }

            present = p;
            rabbit.Add(present);

            previous = rabbit.First();
            rabbit.Remove(previous);

            previous.Clear();
            present.Draw();

            rotate = true;
        } 
        public void Evade()
        {
            Point p = GetPresent();
            //движение по часовой
            switch (direction)
            {
                case Direction.LEFT: //если влево, то вверх
                    p.y -= h;
                    break;
                case Direction.RIGHT: //если вправо, то вниз
                    p.y += h;
                    break;
                case Direction.UP: //если вверх, то вправо
                    p.x += h;
                    break;
                case Direction.DOWN:  //если вниз, то влево
                    p.x -= h;
                    break;
            }

            present = p;
            rabbit.Add(present);

            previous = rabbit.First();
            rabbit.Remove(previous);

            previous.Clear();
            present.Draw();

            rotate = true;
        }

        public bool Eat(List<Point> grass)
        {
            present = GetNextPoint();
            foreach (Point p in grass)
            {
                if (present == p)
                {
                    //rabbit.Add(present);
                    //rabbit.Add(present);
                    //rabbit.Remove(GetPresent());
                    //present = p;
                    rabbit.Add(present);

                    previous = rabbit.First();
                    rabbit.Remove(previous);
                    previous.Clear();
                    present.Draw();
                    grass.Remove(p);
                    hungerLevel++;
                    return true;
                }
            }
            return false;
        }
        public bool CheckForRemove(Point p)
        {
            foreach (Point p_rab in rabbit)
                if (p == p_rab)
                    return true;
            return false;
        }
        public Point GetNextPoint()
        {
            Point p = GetPresent();

            if ((p.x - xgr) < 0)
            {
                direction = Direction.RIGHT;
            }
            else if ((p.x - xgr) > 0)
                direction = Direction.LEFT;
            if ((p.y - ygr) < 0)
            {
                direction = Direction.DOWN;
            }
            else if ((p.y - ygr) > 0)
                direction = Direction.UP;


            switch (direction)
            {
                case Direction.LEFT:
                    p.x -= h;
                    break;
                case Direction.RIGHT:
                    p.x += h;
                    break;
                case Direction.UP:
                    p.y -= h;
                    break;
                case Direction.DOWN:
                    p.y += h;
                    break;
            }
            return p;
        }
        public void Rotation(ConsoleKey key)
        {
            if (rotate)
            {
                switch (direction)
                {
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        if (key == ConsoleKey.DownArrow)
                            direction = Direction.DOWN;
                        else if (key == ConsoleKey.UpArrow)
                            direction = Direction.UP;
                        break;
                    case Direction.UP:
                    case Direction.DOWN:
                        if (key == ConsoleKey.LeftArrow)
                            direction = Direction.LEFT;
                        else if (key == ConsoleKey.RightArrow)
                            direction = Direction.RIGHT;
                        break;
                }
                rotate = false;
            }

        }

        public bool IsHit(Point p)
        {
            for (int i = rabbit.Count - 2; i > 0; i--)
            {

                if (rabbit[i] == p)
                {
                    return true;
                }


            }
            return false;
        }
    }//class rabbit
    class Wolf
    {
        private List<Point> wolf;

        private Direction direction;
        private int h = 1;
        private Point previous;
        private Point present;
        //private int xrab, yrab;
        public Point pointforEat;
        public Rabbit rabbitforEat;

        bool rotate = true;

        public Wolf(int x, int y)
        {
            direction = Direction.RIGHT;
            wolf = new List<Point>();
            Point p = (x, y, '^');
            wolf.Add(p);

            p.Draw();
        }

        public Point GetPresent() => wolf.Last();

        public void Move(List<Point> grass)
        {



            present = GetNextPoint();
            wolf.Add(present);

            previous = wolf.First();

            wolf.Remove(previous);

            previous.Clear();
            foreach (Point gr in grass)
            {
                if (previous == gr)
                {
                    previous = (previous.x, previous.y, '$');
                    previous.Draw();
                }
            }
            present.Draw();

            rotate = true;
        }
        public void MoveToRabbit(Rabbit rabbit)
        {
            Point p = GetPresent();
            int min = 1000;
            foreach (Point rab in rabbit.rabbit)
            {
                if ((Math.Abs(rab.x - p.x) + Math.Abs(rab.y - p.y)) < min)
                {
                    min = Math.Abs(rab.x - p.x) + Math.Abs(rab.y - p.y);
                    pointforEat = rab;
                    rabbitforEat = rabbit;
                }
            }

        }
        public bool Eat(List<Rabbit> rabbits)//(Rabbit rabbit)
        {
            present = wolf.First(); //GetNextPoint();
            if (present == pointforEat)
            {
                Game.WriteLine($"Проверка present({ present.x.ToString()},{present.y.ToString()}) == point({pointforEat.x.ToString()},{pointforEat.y.ToString()})");
                rabbits.Remove(rabbitforEat);
                rabbitforEat = null;
                return true;
            }

            return false;
        }

        public Point GetNextPoint()
        {
            Point p = GetPresent();

            if ((p.x - pointforEat.x) < 0)
            {
                direction = Direction.RIGHT;
            }
            else if ((p.x - pointforEat.x) > 0)
                direction = Direction.LEFT;
            if ((p.y - pointforEat.y) < 0)
            {
                direction = Direction.DOWN;
            }
            else if ((p.y - pointforEat.y) > 0)
                direction = Direction.UP;


            switch (direction)
            {
                case Direction.LEFT:
                    p.x -= h;
                    break;
                case Direction.RIGHT:
                    p.x += h;
                    break;
                case Direction.UP:
                    p.y -= h;
                    break;
                case Direction.DOWN:
                    p.y += h;
                    break;
            }
            return p;
        }
        public void Rotation(ConsoleKey key)
        {
            if (rotate)
            {
                switch (direction)
                {
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        if (key == ConsoleKey.DownArrow)
                            direction = Direction.DOWN;
                        else if (key == ConsoleKey.UpArrow)
                            direction = Direction.UP;
                        break;
                    case Direction.UP:
                    case Direction.DOWN:
                        if (key == ConsoleKey.LeftArrow)
                            direction = Direction.LEFT;
                        else if (key == ConsoleKey.RightArrow)
                            direction = Direction.RIGHT;
                        break;
                }
                rotate = false;
            }

        }

        public bool IsHit(Point p)
        {
            for (int i = wolf.Count - 2; i > 0; i--)
            {

                if (wolf[i] == p)
                {
                    return true;
                }


            }
            return false;
        }
    }//class rabbit

    class GrassFactory
    {
        int x;
        int y;
        char ch;
        public List<Point> grass;

        Random random = new Random();

        public GrassFactory(int x, int y, char ch)
        {
            this.x = x;
            this.y = y;
            this.ch = ch;

            grass = new List<Point>();

            int part, grassCount;
            part = 2;
            grassCount = (int)(x * y * part / 100);
            for (int i = 0; i < grassCount; i++)
            {
                CreateGrass();
            }
        }

        public void CreateGrass()
        {
            Point p = (random.Next(2, x - 2), random.Next(2, y - 2), ch);
            grass.Add(p);
            p.Draw();
        }
    }
}