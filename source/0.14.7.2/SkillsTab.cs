﻿using SevenDaysProfileEditor.GUI;
using SevenDaysSaveManipulator;
using SevenDaysSaveManipulator.GameData;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SevenDaysProfileEditor.Skills
{
    class SkillsTab : TabPage, IInitializable
    {
        private PlayerDataFile playerDataFile;       
        private TableLayoutPanel panel;

        private List<SkillSlot> skillSlots;

        public SkillsTab(PlayerDataFile playerDataFile)
        {
            Text = "Skills";

            this.playerDataFile = playerDataFile;          
        }

        public void Initialize()
        {
            SetUpSkills();

            panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;

            TableLayoutPanel general = new TableLayoutPanel();
            general.Size = new Size(206, 68);
            general.Anchor = AnchorStyles.Top;

            LabeledControl playerLevelBox = new LabeledControl("Player level", new TextBoxInt(playerDataFile.level, 1, SkillData.maxPlayerLevel, 80), 200);
            general.Controls.Add(playerLevelBox);

            LabeledControl skillPointsBox = new LabeledControl("Skill points", new TextBoxInt(playerDataFile.skillPoints, 0, int.MaxValue, 80), 200);
            general.Controls.Add(skillPointsBox);

            panel.Controls.Add(general);

            TableLayoutPanel skillsPanel = new TableLayoutPanel();
            skillsPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            skillsPanel.Dock = DockStyle.Fill;
            skillsPanel.AutoScroll = true;
            skillsPanel.MouseEnter += (sender, e) =>
            {
                bool focusFree = true;
                foreach (SkillSlot skillSlot in skillSlots)
                {
                    if (skillSlot.levelBox.Focused || (skillSlot.expToNextLevelBox != null && skillSlot.expToNextLevelBox.Focused))
                    {
                        focusFree = false;
                        break;
                    }
                }

                if (focusFree)
                {
                    skillsPanel.Focus();
                }
            };

            TableLayoutPanel centerer = new TableLayoutPanel();
            centerer.Anchor = AnchorStyles.Top;
            centerer.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            centerer.AutoSize = true;

            skillSlots = new List<SkillSlot>();
            int i = 0;

            foreach(KeyValuePair<int, Skill> skillEntry in playerDataFile.skills.skillDictionary)
            {
                SkillSlot skillSlot = new SkillSlot(new SkillBinder(skillEntry.Value, playerDataFile.level));

                skillSlots.Add(skillSlot);

                centerer.Controls.Add(skillSlot, i % 4, i / 4);

                i++;
            }

            skillsPanel.Controls.Add(centerer);
            panel.Controls.Add(skillsPanel);

            Controls.Add(panel);
        }

        private void SetUpSkills()
        {
            SkillBinder.SetSkillDicitionary(playerDataFile);

            foreach (SkillData dataSkill in SkillData.skillList)
            {
                bool exists = false;

                foreach (KeyValuePair<int, Skill> skillEntry in playerDataFile.skills.skillDictionary)
                {
                    if (Utils.GetMonoHash(dataSkill.name) == skillEntry.Key)
                    {
                        exists = true;
                    }
                }

                if (!exists)
                {
                    Skill skill = SkillBinder.GetEmptySkill(Utils.GetMonoHash(dataSkill.name), playerDataFile);
                }
            }
        }
    }
}