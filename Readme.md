# Artifact of Focus

**A Risk of Rain 2 mod that introduces a new artifact: the Artifact of Focus.**

## 🧬 What It Does

- 📉 Reduces the spawn rate of:
  - Chests  
  - Multi-shop terminals  
  - Chance shrines

- 🎯 At the start of the run, a single random item is selected as the **focus item**.
- 📦 Every time an item is dropped, **an extra copy** of the focus item is dropped with it.
- 🔁 The selected item remains the same throughout the entire run.

This artifact creates a unique run experience where you build your strategy around a single, randomly chosen item.

Currently there is an issue that i could not fix where printers will also drop the extra item when triggered, this is unintended.
So restrain yourself with printers.
Or not, I'm not your mom.

---

## ⚙️ Configuration

This mod uses **Risk of Options** for in-game configuration.

Available sliders:
- Chest spawn multiplier  
- Terminal store spawn multiplier  
- Chance shrine spawn multiplier  

- Focus item rarity weights:
  - Common (white)
  - Uncommon (green)
  - Rare (red)
  - Boss (yellow)
  - Lunar (blue)

Rarity sliders are not enforced to always sum to 100%, but you should try to set them to sum 100%
What will happen when the sliders are not a total of 100%?
The game will take the total value and use the ratios as the percentages.
For example if you set:
White 100, Green 50 and Red 50, that's 200 instead of 100%.
But the game will treat this as:
White 50%, green 25% and Red 25%, as those are their percentages relative to a total of 200.
So the game doesn't break if it's not a 100 in total, but it's clearer for you if you do.

---

## 📦 Installation

Install with [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager) or manually:

1. Install [BepInExPack](https://thunderstore.io/package/bbepis/BepInExPack/)
2. Install [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/)
3. Install [RiskOfOptions](https://thunderstore.io/package/RiskofThunder/RiskOfOptions/)
4. Download and place `ArtifactOfFocus.dll` in your `BepInEx/plugins/ArtifactOfFocus` folder.

---

## 🛠️ Requirements

- BepInExPack  
- R2API  
- Risk of Options

---

## 💬 !!DISCLAIMER!! / Feedback / Issues

FULL DISCLAIMER This mod was programmed using **ChatGPT** and is likely **janky, or broken** in places. At time of testing everything mostly works, and crash free, but i can't fully test every possible eventuality.
I take no huge amount of pride in this (although it did take me 10 hours to knock this out to a functioning state), but i had an idea and i wanted to see it made real and play it with my friends.
  
I welcome all feedback, help to fix this and especially offers to help clean up, improve, or even fully **adopt this mod as your own**.  
If you want to take over and upload it under your own name, **please do**—you have my full blessing.
Feel free to grab the files or contact me at https://github.com/Basher93/RoR2ArtifactOfFocusMod

## 🛠️ Info for anyone trying to fix or rebuild this mod from scratch:

- At first the script was set up to add the item directly to the inventory when any other item was added.
This proved difficult and caused problems with infinite loops off items adding that needed to be fixed,  i then pivoted to dropping the item in the world.
Unless you know better i recommend staying in this route.

- It also drops an item when a lunar coin is dropped, this proved difficult to fix but inconsequential so i left it in.

- There is a section in the logic dedicated to try to stop printers from also dropping an extra item. This does not work, this problem is not fixed.
For game balance this problem should try to be fixed.
