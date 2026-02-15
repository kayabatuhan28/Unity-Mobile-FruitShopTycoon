# Unity-Mobile-FruitShopTycoon
Fruit Shop Tycoon – Unity Mobile Simulation Game

Due to licensing restrictions, third party assets are excluded from this repository.The project contains scripts and core systems only.This project was built to practice system architecture design in idle/tycoon games.
The main focus was performance optimization through object pooling and clean separation of production validation logic from execution logic.

🎮 Core Gameplay
- The player purchases and plants fruit trees using in game currency.
- Trees produce fruits over time.
- The player collects fruits and places them into juice machines.
- Machines process fruits into fruit juice products.
- Produced juices are served to customers to generate income.

⬆️ Upgrade System
- The game includes multiple progression mechanics:

🌳 Tree Upgrades
- Production speed improvements
- Efficiency scaling

🏭 Machine Upgrades
- Processing speed improvements
- Production efficiency

🧍 Player Upgrades
- Movement speed
- Carry capacity

👥 Customer Upgrades
- Customer capacity
- Flow & queue management improvements

🧠 Technical Features
♻️ Object Pooling System
- To optimize performance on mobile devices:
- Fruits are managed via object pooling instead of Instantiate/Destroy.
- When a fruit is discarded, it returns to the pool.
- Fruits placed into machines are also recycled back into the pool after processing.
- This reduces garbage collection spikes and improves runtime performance.

💾 File-Based Save System
- Player progression is stored using a file-based save/load system.
- Game data is written to and read from persistent storage.
- Runtime data is synchronized via a Load → Apply pattern.

🏗️ Core Systems Architecture
- Slot management system
- Production validation & control system
- Customer queue system
- Payment & cash desk logic
- Data synchronization layer
- State-based production checks

🎯 Focus
- This project emphasizes:
- Clean system architecture
- Scalable naming conventions
- Performance optimization (Pooling)
- Structured data handling

