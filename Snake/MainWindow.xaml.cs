using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        //Поле на котором живет змея
        Entity field;
        // голова змеи
        Head head;
        // вся змея
        List<PositionedEntity> snake;
        // яблоко
        Apple apple;
        //количество очков
        int score;
        //таймер по которому 
        DispatcherTimer moveTimer;
        // кролик
        Rabbit rabbit;
        bool flag = false;
        // количество кроликов(1 или 0)
        int amount_of_rabbits = 0;

        bool flag_for_rotate_rabbit = false;

        int x_rabbit_before = 0;

        //конструктор формы, выполняется при запуске программы
        public MainWindow()
        {
            InitializeComponent();
            
            snake = new List<PositionedEntity>();
            //создаем поле 300х300 пикселей
            field = new Entity(600, 600, "pack://application:,,,/Resources/snake.png");
            
            //создаем таймер срабатывающий раз в 300 мс
            moveTimer = new DispatcherTimer();
            moveTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            grid.Background = new SolidColorBrush(Colors.ForestGreen);
            
        }

        //метод перерисовывающий экран
        private void UpdateField()
        {
            int number = random.Next(0, 20);
            // добавляем кролика
            if (number == 1 && amount_of_rabbits == 0)
            {
                canvas1.Children.Add(rabbit.image);
                flag = true;
                amount_of_rabbits++;
            }
            //обновляем положение элементов змеи
            foreach (var p in snake)
            {
                Canvas.SetTop(p.image, p.y);
                Canvas.SetLeft(p.image, p.x);
            }

            //обновляем положение яблока
            Canvas.SetTop(apple.image, apple.y);
            Canvas.SetLeft(apple.image, apple.x);

            // обновляем положение кролика
            if (flag == true && amount_of_rabbits == 1)
            {
                Canvas.SetTop(rabbit.image, rabbit.y);
                Canvas.SetLeft(rabbit.image, rabbit.x);
            }
            //обновляем количество очков
            lblScore.Content = String.Format("{0}000", score);
        }

        //обработчик тика таймера. Все движение происходит здесь
        void moveTimer_Tick(object sender, EventArgs e)
        {
            //в обратном порядке двигаем все элементы змеи
            foreach (var p in Enumerable.Reverse(snake))
            {
                p.move();
            }
            // передвижение кролика
            if (flag == true && amount_of_rabbits == 1)
            {

                rabbit.move();
                // если змейка врезалась в кролика
                if (head.x == rabbit.x && head.y == rabbit.y)
                {
                    score += 25;
                    amount_of_rabbits--;
                    canvas1.Children.Remove(rabbit.image);
                    rabbit.x = random.Next(13) * 40 + 40;
                    rabbit.y = random.Next(13) * 40 + 40;
                }

                //повороты кролика
                if (rabbit.x > x_rabbit_before || flag_for_rotate_rabbit == false)
                {
                    x_rabbit_before = rabbit.x;
                    rabbit.Rotate = Rabbit.Rabbit_direction.RIGHT;
                    flag_for_rotate_rabbit = true;

                }
                else if (rabbit.x < x_rabbit_before || flag_for_rotate_rabbit == false)
                {
                    x_rabbit_before = rabbit.x;
                    rabbit.Rotate = Rabbit.Rabbit_direction.LEFT;
                    flag_for_rotate_rabbit = true;
                }

                // находим растояние между кроликом и змеёй
                double s_now = Math.Sqrt(Math.Pow(rabbit.x - head.x, 2) + Math.Pow(rabbit.y - head.y, 2));
                // если расстояние между кроликом и змеёй <= 110, то увеличиваем ему скорость и он бежит туда где нет змеии
                if (s_now <= 110)
                {
                    if (rabbit.x > head.x)
                        rabbit.escape(true, false, false, false);
                    else if (rabbit.x < head.x)
                        rabbit.escape(false, true, false, false);

                    if (rabbit.y < head.y)
                        rabbit.escape(false, false, true, false);
                    else if (rabbit.y > head.y)
                        rabbit.escape(false, false, false, true);

                    if (rabbit.x == 520 && head.x == 480 && (rabbit.y != 520 || rabbit.y != 40))
                    {
                        int rand = random.Next(1, 2);
                        if (rand == 1)
                            rabbit.escape(false, false, false, true);
                        else
                            rabbit.escape(false, false, true, false);
                    }
                    else if (rabbit.y == 520 && head.y == 480 && (rabbit.x != 520 || rabbit.x != 40))
                    {
                        int rand1 = random.Next(1, 2);
                        if (rand1 == 1)
                            rabbit.escape(true, false, false, false);
                        else
                            rabbit.escape(false, true, false, false);
                    }
                    else if (rabbit.y == 40 && head.y == 80 && (rabbit.x != 520 || rabbit.x != 40))
                    {
                        int rand2 = random.Next(1, 2);
                        if (rand2 == 1)
                            rabbit.escape(true, false, false, false);
                        else
                            rabbit.escape(false, true, false, false);
                    }
                    else if (rabbit.x == 40 && head.x == 80 && (rabbit.y != 520 || rabbit.y != 40))
                    {
                        int rand3 = random.Next(1, 2);
                        if (rand3 == 1)
                            rabbit.escape(false, false, false, true);
                        else
                            rabbit.escape(false, false, true, false);
                    }
                }

                // столкновение яблока и кролика
                if (rabbit.x == apple.x && rabbit.y == apple.y)
                {
                    apple.move();
                }

                //если кролик врежется в тело змеи, то умрёт
                foreach (var p in snake.Where(x => x != head)) 
                { 
                    if (p.x == rabbit.x && p.y == rabbit.y)
                    {
                        score += 25;
                        amount_of_rabbits--;
                        canvas1.Children.Remove(rabbit.image);
                        rabbit.x = random.Next(13) * 40 + 40;
                        rabbit.y = random.Next(13) * 40 + 40;
                    }
                }
            }       
            //проверяем, что голова змеи не врезалась в тело
            foreach (var p in snake.Where(x => x != head))
            {
                //если координаты головы и какой либо из частей тела совпадают
                if (p.x == head.x && p.y == head.y)
                {
                    //мы проиграли
                    moveTimer.Stop();
                    button1.Visibility = Visibility.Visible;
                    imGameOver.Visibility = Visibility.Visible;
                    return;
                }
            }

            //проверяем, что голова змеи не вышла за пределы поля
            if (head.x < 40 || head.x >= 540 || head.y < 40 || head.y >= 540)
            {
                //мы проиграли
                moveTimer.Stop();
                button1.Visibility = Visibility.Visible;
                imGameOver.Visibility = Visibility.Visible;
                return;
            }

            //проверяем, что голова змеи врезалась в яблоко
            if (head.x == apple.x && head.y == apple.y)
            {
                //увеличиваем счет
                score++;
                //двигаем яблоко на новое место
                apple.move();
                // добавляем новый сегмент к змее
                var part = new BodyPart(snake.Last());
                canvas1.Children.Add(part.image);
                snake.Add(part);
            }
            //перерисовываем экран
            UpdateField();
        }

        // Обработчик нажатия на кнопку клавиатуры
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    head.direction = Head.Direction.UP;
                    break;
                case Key.Down:
                    head.direction = Head.Direction.DOWN;
                    break;
                case Key.Left:
                    head.direction = Head.Direction.LEFT;
                    break;
                case Key.Right:
                    head.direction = Head.Direction.RIGHT;
                    break;
            }
        }

        // Обработчик нажатия кнопки "Start"
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.Visibility = Visibility.Hidden;
            // обнуляем счет
            score = 0;
            // обнуляем змею
            snake.Clear();
            // очищаем канвас
            canvas1.Children.Clear();
            // скрываем надпись "Game Over"
            imGameOver.Visibility = Visibility.Hidden;
            // создаём кролика
            rabbit = new Rabbit();
            // добавляем поле на канвас
            canvas1.Children.Add(field.image);
            // создаем новое яблоко и добавлем его
            apple = new Apple(snake);
            canvas1.Children.Add(apple.image);
            // создаем голову
            head = new Head();
            snake.Add(head);
            canvas1.Children.Add(head.image);
            
            //запускаем таймер
            moveTimer.Start();
            UpdateField();

        }
        
        public class Entity
        {
            protected int m_width;
            protected int m_height;
            
            Image m_image;
            public Entity(int w, int h, string image)
            {
                m_width = w;
                m_height = h;
                m_image = new Image();
                m_image.Source = (new ImageSourceConverter()).ConvertFromString(image) as ImageSource;
                m_image.Width = w;
                m_image.Height = h;

            }

            public Image image
            {
                get
                {
                    return m_image;
                }
            }
        }

        public class PositionedEntity : Entity
        {
            protected int m_x;
            protected int m_y;
            public PositionedEntity(int x, int y, int w, int h, string image)
                : base(w, h, image)
            {
                m_x = x;
                m_y = y;
            }

            public virtual void move() { }

            public int x
            {
                get
                {
                    return m_x;
                }
                set
                {
                    m_x = value;
                }
            }

            public int y
            {
                get
                {
                    return m_y;
                }
                set
                {
                    m_y = value;
                }
            }
        }

        public class Apple : PositionedEntity
        {
            List<PositionedEntity> m_snake;
            public Apple(List<PositionedEntity> s)
                : base(0, 0, 40, 40, "pack://application:,,,/Resources/fruit.png")
            {
                m_snake = s;
                move();
            }

            public override void move()
            {
                Random rand = new Random();
                do
                {
                    x = rand.Next(13) * 40 + 40;
                    y = rand.Next(13) * 40 + 40;
                    bool overlap = false;
                    foreach (var p in m_snake)
                    {
                        if (p.x == x && p.y == y)
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (!overlap)
                        break;
                } while (true);

            }
        }

        public class Rabbit : PositionedEntity
        {
            Random rand = new Random();
            public int num1;

            public enum Rabbit_direction
            {
                RIGHT, LEFT
            };

            public Rabbit_direction Rotate
            {
                set
                {
                    if (value == Rabbit_direction.RIGHT)
                        image.FlowDirection = FlowDirection.LeftToRight;
                    else if (value == Rabbit_direction.LEFT)
                        image.FlowDirection = FlowDirection.RightToLeft;
                }
            }

            public Rabbit() : base(0, 0, 40, 40, "pack://application:,,,/Resources/rabbit_normal.png")
            {
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                x = rand.Next(13) * 40 + 40;
                y = rand.Next(13) * 40 + 40;
            }

            public override void move()
            {
                num1 = rand.Next(1, 5);
                if (y <= 520 && y > 40 && num1 == 1)
                    y -= 40;
                else if (y >= 40 && y < 520 && num1 == 2)
                    y += 40;
                else if (x >= 40 && x < 520 && num1 == 3)
                    x += 40;
                else if (x <= 520 && x > 40 && num1 == 4)
                    x -= 40;
                num1 = 0;

            }

            public void escape(bool flag_for_escape_right, bool flag_for_escape_left, bool flag_for_escape_up, bool flag_for_escape_down)
            {
                if (flag_for_escape_right == true && x >= 40 && x < 520)
                    x += 40;
                else if (flag_for_escape_left == true && x <= 520 && x > 40)
                    x -= 40;
                else if (flag_for_escape_right == true && y >= 40 && y < 520)
                    y += 40;
                else if (flag_for_escape_right == true && y <= 520 && y > 40)
                    y -= 40;
            }
        }
        public class Head : PositionedEntity
        {
            public enum Direction
            {
                RIGHT, DOWN, LEFT, UP, NONE
            };

            Direction m_direction;

            public Direction direction {
                set
                {
                    m_direction = value;
                    RotateTransform rotateTransform = new RotateTransform(90 * (int)value);
                    image.RenderTransform = rotateTransform;
                }
            }

            public Head()
                : base(280, 280, 40, 40, "pack://application:,,,/Resources/head.png")
            {
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                m_direction = Direction.NONE;
            }

            public override void move()
            {
                switch (m_direction)
                {
                    case Direction.DOWN:
                        y += 40;
                        break;
                    case Direction.UP:
                        y -= 40;
                        break;
                    case Direction.LEFT:
                        x -= 40;
                        break;
                    case Direction.RIGHT:
                        x += 40;
                        break;
                }
            }
        }

        public class BodyPart : PositionedEntity
        {
            PositionedEntity m_next;
            public BodyPart(PositionedEntity next)
                : base(next.x, next.y, 40, 40, "pack://application:,,,/Resources/body.png")
            {
                m_next = next;
            }

            public override void move()
            {
                x = m_next.x;
                y = m_next.y;
            }
        }
    }
}