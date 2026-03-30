# EEG-MotorImagery-Unity
A complete closed-loop Brain-Computer Interface (BCI) project to control Unity 3D objects using EEG (Motor Imagery), OpenViBE, and LSL.

This project demonstrates a complete, closed-loop Brain-Computer Interface (BCI). It allows a user to control a 3D object in the Unity game engine using only their mind (Motor Imagery: imagining left or right hand movements). 

The pipeline uses **OpenViBE** for EEG signal processing, Machine Learning (LDA Classifier) for intention detection, and **Lab Streaming Layer (LSL)** for real-time network communication with **Unity**.

---

## 🛠️ Tech Stack & Requirements

1. **Hardware:** An EEG device (4-channels minimum recommended).
2. **Software:**
   * **[OpenViBE](http://openvibe.inria.fr/):** Version 3.7.0 or higher.
   * **[Unity Engine](https://unity.com/):** Version 2022.3 or higher.
   * **Hardware-specific software:** API or software to stream your raw EEG data to the network via LSL.

---

## ⚙️ Step-by-Step Installation Guide

### Step 1: Install and Setup LSL in Unity
To receive brain signals in Unity, you need the LSL package.
1. Open your Unity project.
2. Go to `Window` -> `Package Manager`.
3. Click the `+` icon in the top left and select **"Add package from git URL..."**.
4. Paste the following URL: `https://github.com/labstreaminglayer/LSL4Unity.git` and click Add.
5. Wait for Unity to download and compile the LSL package.

### Step 2: EEG Device Setup
1. Turn on your EEG headset and ensure good electrode contact (low impedance).
2. Open your device's specific streaming software (e.g., C# LSL API app).
3. Start the stream. Ensure the data is being pushed to the network under a specific LSL stream name (e.g., `FlexEEG`).

### Step 3: OpenViBE Setup
1. Download and install [OpenViBE](http://openvibe.inria.fr/downloads/).
2. Open the **OpenViBE Acquisition Server**.
3. In the "Driver" dropdown, select **LabStreamingLayer (LSL)**.
4. Click `Properties` and match the stream name with your EEG software output (e.g., `FlexEEG`).
5. Click **Connect** and then **Play**. You should see the "Receiving and sending..." message.

---

## 🚀 How to Use (The Pipeline)

This project contains three OpenViBE scenarios located in the `OpenViBE_Scenarios` folder. You **must** run them in this exact order to train the AI for your specific brain patterns.

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

If everything is set up correctly, the Unity console will print `Successfully Connected to OpenViBE!`. Think "Left" or "Right" and watch the object move!

---

## 💡 How it Works (GDF Standards)
The communication between OpenViBE and Unity relies on the international General Data Format (GDF) for Brain-Computer Interfaces. When the AI detects a thought, it sends a specific code:
* `769` : `OVTK_GDF_Left` (Left-hand movement imagined)
* `770` : `OVTK_GDF_Right` (Right-hand movement imagined)

The `BCIReader.cs` script in Unity listens for these specific codes and translates them into physical vectors (`Vector3.left` and `Vector3.right`).

---
**Disclaimer:** BCI performance is highly subject-dependent. Factors like electrode placement, muscle artifacts (blinking, jaw clenching), and user focus will drastically affect the accuracy of the classifier.
