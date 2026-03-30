# EEG-MotorImagery-Unity
This project demonstrates a complete, closed-loop Brain-Computer Interface (BCI). It allows a user to control a 3D object in the Unity game engine using only their mind (Motor Imagery: imagining left or right hand movements). 

The pipeline uses **OpenViBE** for EEG signal processing, Machine Learning (LDA Classifier) for intention detection, and **Lab Streaming Layer (LSL)** for real-time network communication with **Unity**.

Created for the MA Game Design Program at Ulster University.

---

## 🛠️ Tech Stack & Requirements

1. **Hardware:** An EEG device (4-channels minimum recommended).
2. **Software:**
   * **[OpenViBE](http://openvibe.inria.fr/):** Version 3.7.0 or higher.
   * **[Unity Engine](https://unity.com/):** Version 2022.3 or higher.
   * **LSL Plugin:** For Unity network communication.

---

## 🎧 Step 1: Hardware Setup (FlexEEG Example)

> 🎓 **CLASS ANNOUNCEMENT (For Enrolled Students):** > The **2 required API files** (C# LSL API) needed to connect the FlexEEG device to your computer will be **provided directly to you in class**. You do not need to search for or download them from the internet. Please have them ready on your machines before proceeding.

> **⚠️ IMPORTANT NOTE FOR OTHER USERS:** > In this tutorial, we are using a **FlexEEG** device with a custom C# Windows API. However, the OpenViBE and Unity sections of this project are **100% hardware-agnostic**. You can use **ANY EEG DEVICE** (e.g., OpenBCI, Emotiv, Muse) as long as it can stream data via LSL.

**Once you have the API files, follow these steps:**

1. Wear the EEG cap and apply conductive gel to the electrodes (ensure they cover the Motor Cortex, e.g., C3 and C4 positions).
2. Connect the device via USB/Bluetooth and open the provided **C# LSL API** application.
3. Select your device's **COM port** and click **`openCOM port`**.
4. Set configurations: **High Pass Filter:** `1 - ON`, **Sampling Rate:** `125Hz`, **Resolution:** `16 bit`.
5. Click **`Start`** to begin data collection (numbers should start fluctuating).
6. Click **`Start LSL`** to broadcast the raw EEG data to your local network (Stream name: `FlexEEG`).

---

## 🎮 Step 2: Install and Setup LSL in Unity

1. Open your Unity project.
2. Go to `Window` -> `Package Manager`.
3. Click the `+` icon in the top-left corner and select **"Add package from git URL..."**.
4. Paste exactly: `https://github.com/labstreaminglayer/LSL4Unity.git` and click **Add**.
5. Attach the `BCIReader.cs` script (provided in this repo) to an empty GameObject in your scene.

---

## 🧠 Step 3: OpenViBE Setup & The Core Pipeline

Download and install [OpenViBE](http://openvibe.inria.fr/downloads/). First, open the **OpenViBE Acquisition Server**, select **LabStreamingLayer (LSL)** as the driver, type your EEG stream name (e.g., `FlexEEG`) in Driver Properties, and click **Connect -> Play**.

Now, open the `OpenViBE_Scenarios` folder from this repository. You **must** run them in this exact order:

### Phase 1: Data Acquisition (`1-acquisition.xml`)
1. Open the scenario in OpenViBE Designer.
2. **[CRUCIAL FIX]:** Double-click the `Graz Motor Imagery BCI Stimulator` box at the bottom. Ensure all timing variables are **whole numbers** (e.g., 1, 4, 2, 3). Decimal numbers can cause the Lua script to crash!
3. Press **Play**. Follow the arrows on the screen and strongly imagine moving your left/right hand.
4. The scenario stops automatically after ~7 minutes, saving a `.ov` file.

### Phase 2: Training the Classifier (`2-classifier-trainer.xml`)
1. Open the scenario.
2. Double-click the **Generic stream reader** box. Click the folder icon and select the `.ov` file you just recorded.
3. Press **Play**. The system trains an LDA classifier and saves a `.cfg` file (your brain's specific model).

### Phase 3: Online Control & Unity Connection (`3-online.xml`)
This is where the magic happens, but it requires precise configuration:

1. Open `3-online.xml`.
2. **Load your Brain Model:** Double-click the `Classifier processor` box. Click the folder icon and select the `.cfg` file created in Phase 2. *(If you don't do this, OpenViBE will use a default 2-channel model and crash with a "BadInput" error because your device has 4 channels).*
3. **The Purple Cable (LSL Export):**
   * If not present, search for the standard **`LSL Export`** box and drag it to the canvas.
   * Right-click it -> **`Modify inputs`**. Change the Type from `Signal` to **`Stimulations`** and click Apply. The input port will turn **Purple**.
   * Connect the **Purple output** of the `Classifier processor` to the **Purple input** of the `LSL Export`.
4. Double-click `LSL Export` and write exactly **`Unity_BCI`** in the `Marker stream` field.
5. Press **Play** in OpenViBE.
6. **Immediately switch to Unity and press Play.**

---

## 💡 How it Works (GDF Standards)
When the AI detects a thought, it sends a standard Medical GDF code over the network:
* `769` : Left-hand movement imagined
* `770` : Right-hand movement imagined
The Unity `BCIReader.cs` script listens for these exact numbers to trigger actions.

---

## 🔥 Common Pitfalls & Troubleshooting (Read Before Asking!)

* **Error: `Classifier expected 2 features, got 4`**
  * **Cause:** You forgot to load your custom `.cfg` file in Phase 3. The default model expects 2 channels, but your device sends 4.
  * **Fix:** Double-click `Classifier processor` and load your `.cfg` file.
* **Error: Lua script terminated immediately on Play**
  * **Cause:** The `Graz Stimulator` box cannot handle floating-point numbers (decimals) in certain regional Windows settings.
  * **Fix:** Open the stimulator box and change all duration variables (like 1.25) to solid integers (like 1, 2, 3, 4).
* **Error: `Stream not found!` in Unity**
  * **Cause 1:** OpenViBE is not currently playing. (OpenViBE *must* be playing and generating the blue bar before you hit Play in Unity).
  * **Cause 2:** The `Marker stream` name in the LSL Export box does not perfectly match the `StreamName` variable in your Unity C# script (`Unity_BCI`).
  * **Cause 3:** You used a green (Signal) wire instead of a purple (Stimulation) wire for the LSL Export box.
