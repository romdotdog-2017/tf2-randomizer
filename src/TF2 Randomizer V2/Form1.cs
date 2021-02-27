using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TF2_Randomizer_V2
{
    public partial class Form1 : Form
    {
        Random r = new Random();

        string weapons = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = textBox1.Text.GetHashCode().ToString();
            r = new Random(textBox1.Text.GetHashCode());
        }

        private void add_log(string text)
        {
            richTextBox1.Text = richTextBox1.Text + text + "\n";
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                add_log("\"" + folderBrowserDialog1.SelectedPath + "\" added to variable \"PATH\"");
                if (!Directory.Exists(folderBrowserDialog1.SelectedPath + @"\weapons"))
                {
                    add_log("[Directory Protection]: This directory is invalid. needs a weapons subdirectory");
                    button2.Enabled = false;
                } else
                {
                    if (Directory.Exists(Path.GetTempPath() + @"\tf2random"))
                    {
                        add_log("[Directory Protection]: Deleting unneeded randomizations from last time..");
                        DeleteDirectory(Path.GetTempPath() + @"\tf2random");
                    }
                    add_log("[Directory Protection]: All clear.");
                    weapons = folderBrowserDialog1.SelectedPath + @"\weapons";
                    button2.Enabled = true;
                }
            }
            
        }

        private void KeyPressedCancel(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.KeyPress += KeyPressedCancel;
            textBox2.Text = r.Next(-2147483647, 2147483647).ToString();
        }

        private string GetRandom(List<string> directories)
        {
            var result = directories[r.Next(0, directories.Count)];
            return result;
        }

        private void CopyDirectory(string SourcePath, string DestinationPath)
        {
            try
            {
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
        SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
            }
            catch
            {
                add_log("Something went wrong..");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Directory.CreateDirectory(Path.GetTempPath() + @"\tf2random");
            Directory.CreateDirectory(Path.GetTempPath() + @"\tf2random\models");
            var actualtemp = Directory.CreateDirectory(Path.GetTempPath() + @"\tf2random\models\weapons").FullName;

            if (check1.Checked)
            {
                add_log("Logging Directories..");
                var directories = Directory.GetDirectories(weapons + @"\c_models").ToList();
                var directories2 = Directory.GetDirectories(weapons + @"\c_models");
                var temp = Directory.CreateDirectory(actualtemp + @"\c_models").FullName;
                foreach (string weeapon in directories2)
                {
                    var opponent = GetRandom(directories);
                    directories.Remove(opponent);
                    var previousname = Path.GetFileName(weeapon);
                    var newname = Path.GetFileName(opponent);
                    var weapon = Directory.CreateDirectory(temp + @"\" + newname).FullName;
                    CopyDirectory(weeapon, weapon);
                    add_log("Doing " + previousname + " -> " + newname);
                    foreach (string file in Directory.GetFiles(weapon))
                    {
                        add_log(Path.GetFileName(file) + " -> " + Path.GetFileName(file).Replace(previousname, newname));
                        var newpath = Path.GetDirectoryName(file) + @"\" + Path.GetFileName(file).Replace(previousname, newname); //On the safe side instead of replacing the entire path
                        File.Move(file, newpath);
                    }
                }

                //TODO: v_models
                add_log("-------------------------\n\n-------------------------");
                add_log("2nd stage: Logging Directories");
                var namefiles = Directory.GetFiles(weapons + @"\v_models", "*.mdl").ToList();
                var namefiles2 = Directory.GetFiles(weapons + @"\v_models", "*.mdl");
                temp = Directory.CreateDirectory(actualtemp + @"\v_models").FullName;
                foreach (string namefile in namefiles2)
                {
                    var opponent = GetRandom(namefiles);
                    var previousname = Path.GetFileName(namefile);
                    var newname = Path.GetFileName(opponent);
                    var files_of_namefile = Directory.GetFiles(weapons + @"\v_models", Path.GetFileNameWithoutExtension(namefile) + "*.*");
                    foreach(string file_of_namefile in files_of_namefile)
                    {
                        add_log(Path.GetFileName(file_of_namefile) + " -> " + Path.GetFileName(file_of_namefile).Replace(previousname, newname));
                        File.Copy(file_of_namefile, temp + @"\" + Path.GetFileName(file_of_namefile).Replace(previousname, newname), true);
                    }
                }

                //TODO: w_models
                add_log("-------------------------\n\n-------------------------");
                add_log("3rd stage: Logging Directories");
                namefiles = Directory.GetFiles(weapons + @"\w_models", "*.mdl").ToList();
                namefiles2 = Directory.GetFiles(weapons + @"\w_models", "*.mdl");
                foreach (string namefile in namefiles2)
                {
                    var opponent = GetRandom(namefiles);
                    var previousname = Path.GetFileName(namefile);
                    var newname = Path.GetFileName(opponent);
                    var files_of_namefile = Directory.GetFiles(weapons + @"\w_models", Path.GetFileNameWithoutExtension(namefile) + "*.*");
                    foreach (string file_of_namefile in files_of_namefile)
                    {
                        add_log(Path.GetFileName(file_of_namefile) + " -> " + Path.GetFileName(file_of_namefile).Replace(previousname, newname));
                        File.Copy(file_of_namefile, temp + @"\" + Path.GetFileName(file_of_namefile).Replace(previousname, newname), true);
                    }
                }
            }
            add_log("Done");
        }
    }
}
