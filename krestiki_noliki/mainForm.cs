using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace krestiki_noliki
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        string huPlayer;
        string aiPlayer;
        List<string> origBoard;
        bool isGameRunning = false;
        Color redBG = Color.OrangeRed;
        //Color greenBG = Color.SeaGreen;
        Color defaultBG = SystemColors.ControlDarkDark;
        //bool isHuPlayerFirst = true;
        private void MainForm_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Выберите параметры и нажмите '" + gameButton.Text + "'.";
        }

        private void PictureBox_DoubleClick(object sender, EventArgs e)
        {
            if (isGameRunning)
            {
                Move bestCell;
                PictureBox elem = (PictureBox)sender;
                if (elem.Image == null)
                {
                    Regex regex = new Regex(@"\D+");
                    int elemIndex;
                    int endSituation;
                    List<string> availCells;
                    string cellNumber = regex.Replace(elem.Name, "");
                    if (int.TryParse(cellNumber, out elemIndex))
                    {
                        origBoard[elemIndex] = huPlayer;
                    }
                    drawMove(huPlayer, elem.Name);
                    availCells = getAvailCellsIndices(origBoard);
                    endSituation = checkForEndSituations(origBoard, huPlayer);
                    if ((endSituation == 0 && availCells.Count == 0) || (endSituation >= 1 && endSituation <= 8))
                    {
                        highlightTerminalSituationCells(endSituation);
                        return;
                    }
                    bestCell = calcBestMove(origBoard, aiPlayer);
                    origBoard[bestCell.index] = aiPlayer;
                    drawMove(aiPlayer, ("pictureBox" + bestCell.index.ToString()));
                    availCells = getAvailCellsIndices(origBoard);
                    endSituation = checkForEndSituations(origBoard, aiPlayer);
                    if ((endSituation == 0 && availCells.Count == 0) || (endSituation >= 1 && endSituation <= 8))
                    {
                        highlightTerminalSituationCells(endSituation);
                        return;
                    }
                }
            }
            else
            {
                statusStrip.BackColor = redBG;
            }
        }

        private void GameButton_Click(object sender, EventArgs e)
        {
            if (!isGameRunning)
            {
                foreach (Control ControlElem in this.panel0.Controls)
                {
                    if (ControlElem.GetType() == typeof(System.Windows.Forms.PictureBox))
                    {
                        ((PictureBox)ControlElem).BackColor = defaultBG;
                        ((PictureBox)ControlElem).Image = null;
                    }
                }
                origBoard = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8" };
                if (huCross.Checked)
                {
                    huPlayer = "X";
                    aiPlayer = "O";
                }
                else
                {
                    huPlayer = "O";
                    aiPlayer = "X";
                }
                if (!huFirstMove.Checked)
                {
                    Move bestCell = calcBestMove(origBoard, aiPlayer);
                    drawMove(aiPlayer, ("pictureBox" + bestCell.index.ToString()));
                    origBoard[(int)bestCell.index] = aiPlayer;
                }
                isGameRunning = true;
                huFirstMove.Enabled = false;
                aiFirstMove.Enabled = false;
                huCross.Enabled = false;
                huNought.Enabled = false;
                gameButton.Text = "Остановить игру";
                statusLabel.Text = "Статус: Игра в процессе.";
                resultLabel.Text = null;
            }
            else
            {
                isGameRunning = false;
                huFirstMove.Enabled = true;
                aiFirstMove.Enabled = true;
                huCross.Enabled = true;
                huNought.Enabled = true;
                gameButton.Text = "Начать игру";
                statusLabel.Text = "Совет: Выберите параметры и нажмите '" + gameButton.Text + "'.";
                resultLabel.Text = null;
            }
            statusStrip.BackColor = SystemColors.Highlight;
        }

        void highlightTerminalSituationCells(int paramTerminalSituation)
        {
            for (int i = 0; i <= 8; i++)
            {
                if (paramTerminalSituation != 0)
                {
                    statusLabel.Text = "Статус: Игра окончена.";
                    resultLabel.Text = "Результат: проигрыш.";
                }
                else
                {
                    statusLabel.Text = "Статус: Игра окончена.";
                    resultLabel.Text = "Результат: ничья.";
                }
                if (paramTerminalSituation == 1)
                {
                    if (i == 0 || i == 1 || i == 2)
                    {
                        sortOutCells(i);
                    }
                }
                else if (paramTerminalSituation == 2)
                {
                    if (i == 3 || i == 4 || i == 5)
                    {
                        sortOutCells(i);
                    }
                }
                else if (paramTerminalSituation == 3)
                {
                    if (i == 6 || i == 7 || i == 8)
                    {
                        sortOutCells(i);
                    }
                }
                else if (paramTerminalSituation == 4)
                {
                    if (i == 0 || i == 3 || i == 6)
                    {
                        sortOutCells(i);
                    }
                }
                else if (paramTerminalSituation == 5)
                {
                    if (i == 1 || i == 4 || i == 7)
                    {
                        sortOutCells(i);
                    }
                }
                else if (paramTerminalSituation == 6)
                {
                    if (i == 2 || i == 5 || i == 8)
                    {
                        sortOutCells(i);
                    }
                }
                else if (paramTerminalSituation == 7)
                {
                    if (i == 0 || i == 4 || i == 8)
                    {
                        sortOutCells(i);
                    }
                }
                else if (paramTerminalSituation == 8)
                {
                    if (i == 2 || i == 4 || i == 6)
                    {
                        sortOutCells(i);
                    }
                }
            }
            isGameRunning = false;
            huFirstMove.Enabled = true;
            aiFirstMove.Enabled = true;
            huCross.Enabled = true;
            huNought.Enabled = true;
            gameButton.Text = "Начать игру";
        }

        List<string> getAvailCellsIndices(List<string> paramBoard)
        {
            List<string> emptyCells = new List<string> { };
            foreach (string item in paramBoard)
            {
                if (item != aiPlayer && item != huPlayer)
                {
                    emptyCells.Add(item);
                }
            }
            return emptyCells;
        }

        void sortOutCells(int cellNumber)
        {
            foreach (Control ControlElem in this.panel0.Controls)
            {
                if (ControlElem.GetType() == typeof(System.Windows.Forms.PictureBox))
                {
                    if (((PictureBox)ControlElem).Name == ("pictureBox" + cellNumber))
                    {
                        ((PictureBox)ControlElem).BackColor = redBG;
                    }
                }
            }
        }

        void drawMove(string paramPlayer, string paramCellName)
        {
            foreach (Control ControlElem in this.panel0.Controls)
            {
                if (ControlElem.GetType() == typeof(System.Windows.Forms.PictureBox))
                {
                    if (((PictureBox)ControlElem).Name == paramCellName)
                    {
                        ((PictureBox)ControlElem).Load(paramPlayer == "X" ? ("krestik.png") : ("nolik.png"));
                        ((PictureBox)ControlElem).SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }

        Move calcBestMove(List<string> paramBoard, string paramPlayer)
        {
            List<string> availCells = getAvailCellsIndices(paramBoard);
            if (checkForEndSituations(paramBoard, huPlayer) != 0)
            {
                Move moveObj = new Move();
                moveObj.score = -10;
                return moveObj;
            }
            else if (checkForEndSituations(paramBoard, aiPlayer) != 0)
            {
                Move moveObj = new Move();
                moveObj.score = 10;
                return moveObj;
            }
            else if (availCells.Count == 0)
            {
                Move moveObj = new Move();
                moveObj.score = 0;
                return moveObj;
            }

            List<Move> movesList = new List<Move> { };
            for (int i = 0; i < availCells.Count; i++)
            {
                Move moveObj = new Move();
                if (int.TryParse(availCells[i], out moveObj.index))
                {
                    paramBoard[moveObj.index] = paramPlayer;
                    if (paramPlayer == aiPlayer)
                    {
                        Move result = calcBestMove(paramBoard, huPlayer);
                        moveObj.score = result.score;
                    }
                    else
                    {
                        Move result = calcBestMove(paramBoard, aiPlayer);
                        moveObj.score = result.score;
                    }
                    paramBoard[moveObj.index] = moveObj.index.ToString();
                    movesList.Add(moveObj);
                }
            }

            List<int> bestMovesCellIndicesList = new List<int> { };
            int bestScore;
            if (paramPlayer == aiPlayer)
            {
                bestScore = -10000;
                for (int i = 0; i < movesList.Count; i++)
                {
                    if (movesList[i].score > bestScore)
                    {
                        bestScore = movesList[i].score;
                    }
                }
            }
            else
            {
                bestScore = 10000;
                for (int i = 0; i < movesList.Count; i++)
                {
                    if (movesList[i].score < bestScore)
                    {
                        bestScore = movesList[i].score;
                    }
                }
            }
            for (int i = 0; i < movesList.Count; i++)
            {
                if (movesList[i].score == bestScore)
                {
                    bestMovesCellIndicesList.Add(i);
                }
            }

            int randomMoveCellIndex;
            Random rnd = new Random();
            randomMoveCellIndex = (int)Math.Floor(rnd.NextDouble() * bestMovesCellIndicesList.Count);
            return movesList[bestMovesCellIndicesList[randomMoveCellIndex]];
        }

        int checkForEndSituations(List<string> paramBoard, string paramPlayer)
        {
            if (paramBoard[0] == paramPlayer && paramBoard[1] == paramPlayer && paramBoard[2] == paramPlayer)
            {
                return 1;
            }
            else if (paramBoard[3] == paramPlayer && paramBoard[4] == paramPlayer && paramBoard[5] == paramPlayer)
            {
                return 2;
            }
            else if (paramBoard[6] == paramPlayer && paramBoard[7] == paramPlayer && paramBoard[8] == paramPlayer)
            {
                return 3;
            }
            else if (paramBoard[0] == paramPlayer && paramBoard[3] == paramPlayer && paramBoard[6] == paramPlayer)
            {
                return 4;
            }
            else if (paramBoard[1] == paramPlayer && paramBoard[4] == paramPlayer && paramBoard[7] == paramPlayer)
            {
                return 5;
            }
            else if (paramBoard[2] == paramPlayer && paramBoard[5] == paramPlayer && paramBoard[8] == paramPlayer)
            {
                return 6;
            }
            else if (paramBoard[0] == paramPlayer && paramBoard[4] == paramPlayer && paramBoard[8] == paramPlayer)
            {
                return 7;
            }
            else if (paramBoard[2] == paramPlayer && paramBoard[4] == paramPlayer && paramBoard[6] == paramPlayer)
            {
                return 8;
            }
            else
            {
                return 0;
            }
        }
    }

    class Move
    {
        public int score;
        public int index;
        public Move()
        {
            this.score = -1;
            this.index = -1;
        }
    }
}