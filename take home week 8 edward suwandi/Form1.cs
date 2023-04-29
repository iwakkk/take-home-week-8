using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace take_home_week_8_edward_suwandi
{
    public partial class Form1 : Form
    {
        public static string sqlConnection = "server=139.255.11.84;uid=student;pwd=isbmantap;database=db_punyaiwak";
        public MySqlConnection sqlConnect = new MySqlConnection(sqlConnection);
        public MySqlCommand sqlCommand;
        public MySqlDataAdapter sqlAdapter;
        public string sqlQuery;

        DataTable teamnyaiwak = new DataTable();
        DataTable playernyaiwak = new DataTable();
        DataTable datanyaiwak = new DataTable();

        DataTable datamatchiwak = new DataTable();
        DataTable datadgviwak = new DataTable();
        DataTable datadgviwak2 = new DataTable();


        string simpandata = "babi";
        string simpandata2 = "babi";

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlQuery = "SELECT team_id, team_name from team;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(teamnyaiwak);
            comboBox1.DataSource = teamnyaiwak;
            comboBox1.DisplayMember = "team_name";
            comboBox1.ValueMember = "team_id";
            comboBox3.DataSource = teamnyaiwak;
            comboBox3.DisplayMember = "team_name";
            comboBox3.ValueMember = "team_id";
            comboBox2.Enabled = false;
            comboBox4.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;


        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            playernyaiwak.Rows.Clear();
            sqlQuery = "select player_name, player_id\r\nfrom player\r\nwhere player.team_id = '" + comboBox1.SelectedValue + "';";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(playernyaiwak);
            comboBox2.DataSource = playernyaiwak;
            comboBox2.DisplayMember = "player_name";
            comboBox2.ValueMember = "player_id";
            comboBox2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            simpandata = comboBox2.SelectedValue.ToString();
            datanyaiwak.Rows.Clear();
            sqlQuery = "select p.player_name, t.team_name, p.team_number, n.nation, p.playing_pos, sum(if(type = 'CY', 1,0)) as YellowCard, sum(if(type = 'CR',1,0)) as RedCard, sum(if(type = 'GO',1,0)) as Goal, sum(if(type = 'PM',1,0)) as PenaltyMissed from player p left join dmatch d on p.player_id = d.player_id join nationality n on p.nationality_id = n.nationality_id join team t on p.team_id = t.team_id where p.player_id = '" + simpandata + "' group by player_name;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(datanyaiwak);
            lblname.Text = datanyaiwak.Rows[0][0].ToString();
            lblteam.Text = datanyaiwak.Rows[0][1].ToString();
            lblpos.Text = datanyaiwak.Rows[0][2].ToString();
            lblnasionality.Text = datanyaiwak.Rows[0][3].ToString();
            lblplayingpos.Text = datanyaiwak.Rows[0][4].ToString();
            lblyc.Text = datanyaiwak.Rows[0][5].ToString();
            lblrc.Text = datanyaiwak.Rows[0][6].ToString();
            lblgoal.Text = datanyaiwak.Rows[0][7].ToString();
            lblpenalty.Text = datanyaiwak.Rows[0][8].ToString();

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void showMatchDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Hide();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Enabled = true;
            datamatchiwak.Rows.Clear();
            simpandata2 = comboBox3.SelectedValue.ToString();
            sqlQuery = "select concat(t1.team_name , \" vs \", t2.team_name) as pertandingan, m.match_id from team t1 , team t2, `match` m where t2.team_id = m.team_away and t1.team_id = m.team_home and t1.team_id = '" + simpandata2 + "' union  select concat(t1.team_name , \" vs \", t2.team_name) as pertandingan, m.match_id from team t1 , team t2, `match` m where t2.team_id = m.team_away and t1.team_id = m.team_home and t2.team_id = '" + simpandata2+"';\r\n";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(datamatchiwak);
            comboBox4.DataSource = datamatchiwak;
            comboBox4.DisplayMember = "pertandingan";
            comboBox4.ValueMember = "match_id";


        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            datadgviwak.Rows.Clear();
            datadgviwak2.Rows.Clear();
            sqlQuery = "select p.player_name, t.team_name, p.playing_pos from team t , player p, `match` m where t.team_id = m.team_away and t.team_id = p.team_id and m.match_id = '"+comboBox4.SelectedValue+ "'union select p.player_name, t.team_name, p.playing_pos from team t , player p, `match` m where t.team_id = m.team_home and t.team_id = p.team_id and m.match_id = '" + comboBox4.SelectedValue+"';";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(datadgviwak);
            dataGridView1.DataSource = datadgviwak;

            sqlQuery = $"select d.minute, t.team_name, p.player_name, if(d.type = 'CY' ,'yellow card', if(d.type = 'GO', 'Goal', if(d.type = 'GW', 'Own Goal', if(d.type = 'CR', 'Red Card', if (d.type = 'PM', 'Penalty Miss', 'Goal Penalty'))))) as type from dmatch d, team t, player p where d.match_id = '"+ comboBox4.SelectedValue + "' and d.team_id = t.team_id and p.player_id = d.player_id;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(datadgviwak2);
            dataGridView2.DataSource = datadgviwak2;


        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            this.panel1.BringToFront();
        }

        private void playerDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Show();
        }
    }
}
