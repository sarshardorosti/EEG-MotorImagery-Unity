# EEG-MotorImagery-Unity
*Created for the MA Game Design Program at Ulster University.*

This project demonstrates a complete, closed-loop Brain-Computer Interface (BCI). It allows a user to control a 3D object in the Unity game engine using only their mind (Motor Imagery: imagining left or right hand movements). 

The pipeline uses **OpenViBE** for EEG signal processing, Machine Learning (LDA Classifier) for intention detection, and **Lab Streaming Layer (LSL)** for real-time network communication with **Unity**.

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

> **⚠️ IMPORTANT NOTE FOR OTHER USERS:** > In this tutorial, we are using a **FlexEEG** device with a custom C# Windows API. However, the OpenViBE and Unity sections of this project are **100% hardware-agnostic**. You can use **ANY EEG DEVICE** (e.g., OpenBCI, Emotiv, Muse, g.tec) as long as it can stream data via LSL.

**Once you have the API files (FlexEEG), follow these steps to start the brainwave stream:**

1. Wear the EEG cap and apply conductive gel to the electrodes (ensure they cover the Motor Cortex, e.g., C3 and C4 positions).
2. Connect the device via USB/Bluetooth and open the provided **C# LSL API** application.
3. Select your device's **COM port** (e.g., COM6) and click the **`openCOM port`** button.
4. Set the hardware configurations:
   * **High Pass Filter:** `1 - ON`
   * **Sampling Rate:** `125Hz`
   * **Resolution:** `16 bit`
   * **Enabled Channels:** `4` (or match your physical setup)
5. Click **`Start`** to begin data collection (numbers should start fluctuating on the screen).
6. Click **`Start LSL`** to broadcast the raw EEG data to your local network. (The stream is now live under the name `FlexEEG`).

---

## 🎮 Step 2: Install and Setup LSL in Unity

To receive brain signals in Unity, you need the official LSL package.
1. Open your Unity project.
2. Go to `Window` -> `Package Manager`.
3. Click the `+` icon in the top-left corner and select **"Add package from git URL..."**.
4. Paste the following URL: `https://github.com/labstreaminglayer/LSL4Unity.git` and click **Add**.
5. Wait for Unity to download and compile the LSL package.
6. Attach the `BCIReader.cs` script (provided in this repo) to an empty GameObject in your scene.

---

## 🧠 Step 3: OpenViBE Setup & Connection

Now we need to catch the LSL stream sent by the EEG device and bring it into OpenViBE for processing.
1. Download and install [OpenViBE](http://openvibe.inria.fr/downloads/).
2. Open the **OpenViBE Acquisition Server**.
3. In the "Driver" dropdown, select **LabStreamingLayer (LSL)**.
4. Click `Driver Properties` and type the exact name of your EEG stream (e.g., `FlexEEG`).
5. Click **Connect** and then **Play**. You should see a green bar and the message `"Receiving and sending... 1 client connected"`.

---

## 🚀 Step 4: How to Use (The Core Pipeline)

This repository contains three OpenViBE XML scenarios in the `OpenViBE_Scenarios` folder. You **must** run them in this exact order to train the Artificial Intelligence for your specific brain patterns.

### Phase 1: Data Acquisition (`1-acquisition.xml`)
We need to record your brainwaves while you imagine moving your hands.
1. Open `1-acquisition.xml` in OpenViBE Designer.
2. Press **Play**. A black window with a blue cross will appear.
3. Follow the arrows on the screen. When a left arrow appears, strongly imagine clenching your left hand. Do the same for the right.
4. The scenario will automatically stop after ~7 minutes. Your raw data is saved as a `.ov` file.

### Phase 2: Training the Classifier (`2-classifier-trainer.xml`)
Now we teach the machine learning model to understand your specific brainwaves.
1. Open `2-classifier-trainer.xml`.
2. Double-click the **Generic stream reader** box and select the `.ov` file you just recorded.
3. Press **Play**. The system will train an LDA classifier in seconds.
4. Check the console for your "Cross-validation accuracy". 
5. The model is automatically saved as a `.cfg` file.

### Phase 3: Online Control & Unity Connection (`3-online.xml`)
Let's connect the brain to the game!
1. Open `3-online.xml`.
2. Double-click the **Classifier processor** box and load your newly created `.cfg` file.
3. Make sure the output of the classifier is connected to the **LSL Export** box.
4. Double-click **LSL Export** and ensure the `Marker stream` is named exactly **`Unity_BCI`**.
5. Press **Play** in OpenViBE.
6. **Immediately switch to Unity and press Play.**

If everything is set up correctly, the Unity console will print `Successfully Connected to OpenViBE!`. Think "Left" or "Right" and watch the object move in Unity!

---

## 💡 How it Works (GDF Standards)
The communication between OpenViBE and Unity relies on the international General Data Format (GDF) for Brain-Computer Interfaces. When the AI detects a thought, it sends a specific code:
* `769` : `OVTK_GDF_Left` (Left-hand movement imagined)
* `770` : `OVTK_GDF_Right` (Right-hand movement imagined)

The `BCIReader.cs` script in Unity listens for these specific codes and translates them into physical movement (`Vector3.left` and `Vector3.right`).

---
**Disclaimer:** BCI performance is highly subject-dependent. Factors like electrode placement, muscle artifacts (blinking, jaw clenching), and user focus will drastically affect the accuracy of the classifier. A model trained on Person A's brain will not work well for Person B.
