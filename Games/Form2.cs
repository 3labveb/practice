using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Games
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

            int width = 10;  //ширина поля
            int height = 10; //высота поля
            int offset = 30; //размер ячейки
            int bombPercent = 20;  //процент бомб на поле
            bool isFirstClick = true;

            FieldButton[,] field;
            int openedCells = 0;
            int bombs = 0;


        public void GenerateField()
            {
                Random random = new Random();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        FieldButton newButton = new FieldButton();
                        newButton.Location = new Point(x * offset, y * offset);
                        newButton.Size = new Size(offset, offset);
                        newButton.isClickable = true;
                        if (random.Next(0, 100) <= bombPercent)
                        {
                            newButton.isBomb = true;
                            bombs++;
                        }
                        newButton.xCoord = x;
                        newButton.yCoord = y;
                        Controls.Add(newButton);
                        newButton.MouseUp += new MouseEventHandler(FieldButtonClick);
                        field[x, y] = newButton;
                    }
                }
            }

            void FieldButtonClick(object sender, MouseEventArgs e)
            {
                FieldButton clickedButton = (FieldButton)sender;
                if (e.Button == MouseButtons.Left && clickedButton.isClickable)
                {
                    if (clickedButton.isBomb)
                    {
                        if (isFirstClick)
                        {
                            clickedButton.isBomb = false;
                            isFirstClick = false;
                            bombs--;
                            OpenRegion(clickedButton.xCoord, clickedButton.yCoord, clickedButton);
                        }
                        else
                        {
                            Boom();
                        }
                    }
                    else
                    {
                        EmptyFieldButtonClick(clickedButton);
                    }
                    isFirstClick = false;
                }
                if (e.Button == MouseButtons.Right)
                {
                    clickedButton.isClickable = !clickedButton.isClickable;
                    if (!clickedButton.isClickable)
                    {
                        clickedButton.Text = "B";
                    }
                    else
                    {
                        clickedButton.Text = "";
                    }
                }
                Victory();
            }

            void Boom()
            {
                foreach (FieldButton button in field)
                {
                    if (button.isBomb)
                    {
                        button.Text = "*";
                    }
                }
                MessageBox.Show("Вы проиграли :(");
            }
            void EmptyFieldButtonClick(FieldButton clickedButton)
            {

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (field[x, y] == clickedButton)
                        {
                            OpenRegion(x, y, clickedButton);
                        }
                    }
                }
            }

            void OpenRegion(int xCoord, int yCoord, FieldButton clickedButton)
            {
                Queue<FieldButton> queue = new Queue<FieldButton>();
                queue.Enqueue(clickedButton);
                clickedButton.wasAdded = true;
                while (queue.Count > 0)
                {
                    FieldButton currentCell = queue.Dequeue();
                    OpenCell(currentCell.xCoord, currentCell.yCoord, currentCell);
                     openedCells++;
                    if (CountBombsAround(currentCell.xCoord, currentCell.yCoord) == 0)
                    {
                        for (int y = currentCell.yCoord - 1; y <= currentCell.yCoord + 1; y++)
                        {
                            for (int x = currentCell.xCoord - 1; x <= currentCell.xCoord + 1; x++)
                            {
                                if (x == currentCell.xCoord && y == currentCell.yCoord)
                                {
                                    continue;
                                }
                                if (x >= 0 && x < width && y < height && y >= 0)
                                {
                                    if (!field[x, y].wasAdded)
                                    {
                                        queue.Enqueue(field[x, y]);
                                        field[x, y].wasAdded = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            void OpenCell(int x, int y, FieldButton clickedButton)
            {
                int bombsAround = CountBombsAround(x, y);
                if (bombsAround == 0)
                {

                }
                else
                {
                    clickedButton.Text = "" + bombsAround;
                }
                clickedButton.Enabled = false;
            }
            int CountBombsAround(int xCoord, int yCoord)
            {
                int bombsAround = 0;
                for (int x = xCoord - 1; x <= xCoord + 1; x++)
                {
                    for (int y = yCoord - 1; y <= yCoord + 1; y++)
                    {
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            if (field[x, y].isBomb == true)
                            {
                                bombsAround++;
                            }
                        }
                    }
                }
                return bombsAround;
            }

            void Victory()
            {
                int cells = width * height;
                int emptycells = cells - bombs;
                if (openedCells == emptycells)
                {
                    MessageBox.Show("Вы победили!");
                }
            }

        private void Form2_Load(object sender, EventArgs e)
        {
            field = new FieldButton[width, height];
            GenerateField();
        }
    }

    public class FieldButton : Button
        {
            public bool isBomb;
            public bool isClickable;
            public bool wasAdded;
            public int xCoord;
            public int yCoord;
        }
    }

