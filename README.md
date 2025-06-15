# 🛍️ Project Overview
## Ecommerce Control Management System

This project is an **Ecommerce Control Management System** built specifically with small and medium-sized businesses in mind. It provides a streamlined, intuitive environment for managing products, orders, discounts — all from a centralized admin panel.

The platform is designed to give business owners maximum flexibility: most of the store content and behavior can be adjusted independently, without requiring technical expertise. From configuring product categories and prices to handling discounts and shipping options, the system puts full control into the hands of the shop owner.

Beyond simple product management, the system ensures complete oversight of the **entire order lifecycle** — including payment status, stock synchronization, and automated cleanup of abandoned orders. It's a practical toolset tailored to real-world needs, with usability and customization at its core.

## ✨ Features & Functionalities

### 📁 Category Management

The administrator has full control over building and organizing the shop’s category structure. The system supports **nested categories**, allowing you to create logical trees such as:

Cars
├── SUVs
└── Buses

sql
Copy
Edit

To ensure a clear and user-friendly search experience, there is an intentional restriction in place:  
➡️ **Only top-level categories can contain products.**

This means:
- If a category already contains products, you cannot add subcategories to it.
- If a category has subcategories, it cannot directly contain products.

This rule prevents confusion during product discovery and ensures that users don’t miss items hidden in overly general categories.

The category system is designed to:
- Encourage clean and meaningful hierarchy
- Improve search and filtering accuracy
- Avoid dead-end categories with unreachable products
