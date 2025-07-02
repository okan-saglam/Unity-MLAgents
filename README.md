
# Predator-Prey Multi-Agent RL Environment

A Unity ML-Agents simulation where two agents — a **Pellet Collector (Prey)** and a **Hunter** — interact in a shared environment. The prey collects randomly spawned pellets while avoiding being caught by the hunter. The hunter is rewarded for catching the prey, while the prey is rewarded for successfully collecting all pellets before being caught or timed out.

---

## Features

- **Reinforcement Learning-based control** using [Unity ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents).
- **Two agents** trained concurrently with competitive objectives.
- **Prey**: collect all pellets to win.
- **Hunter**: catch the prey before time runs out or all pellets are collected.
- **Time-based episode endings** with penalty for stalling.
- **Pellet spawning algorithm** that avoids spatial overlaps using proximity constraints.
- **Environment feedback** through color changes (green, red, blue, yellow) based on outcome.
- Supports both **training and heuristic (manual)** control.

---

## Agent Roles & Reward System

| Agent | Objective | Reward Strategy |
|-------|-----------|-----------------|
| **Pellet Collector (Prey)** | Collect all spawned pellets | +10 per pellet, +5 on full collection, -13 if caught, -15 if timeout occurs, -15 if it touches walls |
| **Hunter** | Catch the pellet collector | +10 if successful, -15 if failed (timeout or prey wins or touches walls) |

---

## Environment Color Feedback

The ground material color provides immediate visual feedback for the episode outcome:

| Color | Meaning |
|-------|---------|
| 🟩 **Green** | All pellets collected by the prey agent |
| 🟦 **Blue** | Time ran out before prey could collect all pellets |
| 🟨 **Yellow** | Prey was caught by the hunter |
| 🟥 **Red** | Either agent collided with a wall (penalized and episode ended) |

These color transitions help both developers and observers understand episode outcomes without inspecting logs or training data.

---

## Training Setup

Ensure you have the [ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents) installed and configured.

```bash
mlagents-learn config/trainer_config.yaml --run-id=DualAgentRun1
```

You can control each agent's configuration independently via the YAML file.

---

## Media

### Training Progress  
[![Watch on YouTube](https://img.youtube.com/vi/loYZeJFcyI8/0.jpg)](https://www.youtube.com/watch?v=loYZeJFcyI8)  
▶️ *Click to watch training progress on YouTube*

### Trained Agents in Action  
[![Watch on YouTube](https://img.youtube.com/vi/Pilrd2wEAVA/0.jpg)](https://www.youtube.com/watch?v=Pilrd2wEAVA)  
▶️ *Click to watch trained agents in action*


---

## Requirements

- Unity 2021.x or later
- ML-Agents Toolkit v2.x
- Python 3.8+
- `requirements.txt` file was generated from the virtual environment
  
---

## Run the Simulation

- Open the project in Unity.
- Load the training scene.
- Press **Play** to control manually (Heuristic mode) or run training with `mlagents-learn`.
