# 🛍️ Project Overview
## Ecommerce Control Management System

This project is an **Ecommerce Control Management System** built specifically with small and medium-sized businesses in mind. It provides a streamlined, intuitive environment for managing products, orders, discounts — all from a centralized admin panel.

The platform is designed to give business owners maximum flexibility: most of the store content and behavior can be adjusted independently, without requiring technical expertise. From configuring product categories and prices to handling discounts and shipping options, the system puts full control into the hands of the shop owner.

Beyond simple product management, the system ensures complete oversight of the **entire order lifecycle** — including payment status, stock synchronization, and automated cleanup of abandoned orders. It's a practical toolset tailored to real-world needs, with usability and customization at its core.

## ✨ Features & Functionalities

### 📁 Category Management

The administrator has full control over building and organizing the shop’s category structure. The system supports **nested categories**, allowing you to create logical category trees

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
<p align="center">
  <img src="https://github.com/user-attachments/assets/f10618b7-7f5c-49d0-b9d9-9edbd22338ad" alt="Image 1" width="45%"  style="margin-right: 10px;"/>
  <img src="https://github.com/user-attachments/assets/599df214-6c55-40ae-9bcf-3e36c03422e8" alt="Image 2" width="45%" />
</p>

### 🚚 Carrier Management

The system offers flexible delivery management through a dedicated **Carrier Management** panel.

Administrators can:
- Add and manage custom carriers
- Define whether the shipping price is **fixed** or calculated **per kilogram**
- Set a **free shipping threshold** (e.g., free shipping for orders above $100)
- Enable or disable carriers as needed

This allows business owners to fully customize delivery pricing according to product weight, pricing strategy, or promotional campaigns.

<p align="center">
  <img src="https://github.com/user-attachments/assets/96e307f8-f839-4b1f-a121-926437350c85" alt="Carrier Management Panel" width="45%" style="margin-right: 10px;" />
  <img src="https://github.com/user-attachments/assets/22c024af-8932-4646-b034-0fc47826ddd7" alt="Carrier Pricing Options" width="45%" />
</p>

### 🛍️ Product & Discount Management

The admin panel allows you to create and manage detailed product entries with full control over content, media, and pricing behavior.

Admins can:
- Set product name, price, stock, and category
- Write rich product descriptions using a built-in editor
- Upload up to **7 photos** for each product  
  - The **first photo** is treated as the **main product image**
  - Additional photos appear on the **product details page**
  - You can **easily change** which uploaded image is set as the main

You also have access to a powerful **shipping price factor** mechanism.

> ℹ️ **Shipping Price Factor**  
> This value is used to calculate shipping cost and defaults to the product's weight.  
> It becomes especially useful when:
> - The product is large but lightweight — you can increase the factor
> - The selected carrier charges per kilogram
> - You want to influence the shipping cost for specific items manually  
> 
> 💡 Example:  
> If a carrier charges $5 per kg and a product has a shipping factor of 2.5, the total shipping cost is $12.50.

#### 🎯 Discount Management

Discounts can be added in two ways:
- While **creating a new product**
- By **editing any existing product**

The admin can define:
- Discount **percentage**
- **Start** and **end date** (including time)
- Easily remove or overwrite the discount with one click

This flexible system allows you to plan future promotions, flash sales, or seasonal deals in advance.

<p align="center">
  <img src="https://github.com/user-attachments/assets/d9d20f6e-c17c-4e0d-b30b-388c5fddadad" alt="Discount Scheduling" width="60%" />
</p>




