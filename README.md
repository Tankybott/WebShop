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
  <img src="https://github.com/user-attachments/assets/0d0cb7a1-dcce-485c-8f74-31720180d3e4" alt="Discount Scheduling" width="60%" />
</p>

### ⚙️ Webshop Settings

In the **Webshop Settings** panel, you can configure:

- The **name of your store** (displayed across the platform)
- The **currency** used for all orders and price displays

> 💡 Make sure the selected currency is supported by Stripe if you plan to enable online payments.


### 👥 User Management

The system includes a simple but effective **User Management** panel.

As a head admin, you can:
- **Block or unblock users** to restrict access without deleting their account
- **Create new admin accounts** to share management responsibilities with other trusted users

This feature ensures that access control remains in your hands while maintaining flexibility for team-based administration.

Great — here's the Order Management section rewritten to perfectly match your existing Markdown structure and style:

md
Copy
Edit
### 📦 Order Management

The system provides a comprehensive **Order Management** experience tailored for both regular users and administrators.

---

#### 👤 User Order Access

Each user has access to a personal **My Orders** dashboard, where they can:

- View a list of their past orders
- See details like:
  - Order date
  - Order status
  - Payment status
  - Total price
- Click on any order to see its full summary:
  - Ordered items (name, quantity, price)
  - Shipping method and delivery status

This section allows users to monitor their purchases and get clear visibility into the fulfillment process.

---

#### 🛠️ Admin Order Management

Administrators can manage the full lifecycle of all orders via the **Admin Order Panel**.

They can:

- View all orders in a searchable and filterable table
- Click on any order to:
  - Edit customer and shipping details
  - See payment information
  - Update the order status

The admin interface includes the following editable fields:

- Name, Phone, Address, City, Region, Postal Code
- Email
- Order Date
- **Carrier**
- **Tracking Link**
- **Shipping Date**
- Stripe Session ID
- Payment Intent ID
- Payment Date
- **Order Status** (`Created`, `WaitingForPayment`, `Processing`, `Shipped`, etc.)

> ⚠️ **Important Requirement Before Marking as Shipped**
> 
> Before changing the order status to `Shipped`, the following fields must be properly filled out:
> 
> - ✅ **Carrier**
> - ✅ **Tracking Link**
> - ✅ **Shipping Date** (valid and not left as default)

If any of these fields are missing, the system should block the update and inform the admin.

---

#### 📄 Order PDF Export

Each order has a **Download PDF** button available in the admin panel.

The generated PDF includes:

- Customer and shipping details
- Order summary table:
  - Product name
  - Quantity
  - Price per item
- Total amount (with optional currency formatting)
- Optional tracking and carrier info

This allows for easy archiving or order sharing for administrative or logistical purposes.

<p align="center">
  <img src="https://github.com/user-attachments/assets/ce1359bb-e0bf-402d-8448-d739403aea74" alt="Order Summary Admin Panel" width="80%" />
</p>


### 🔄 Stock & Price Lifecycle

Due to the dynamic nature of pricing, discounts, and stock levels, the system includes built-in validation and adjustment mechanisms to ensure consistency and avoid overselling.

Here's how it works:

- 🛒 **Cart Validation on Entry**  
  Each time a user opens their cart, the system re-validates the **current price and stock** of each product.  
  If anything has changed (e.g., product went on sale, stock decreased), the user is immediately informed, and the cart is automatically adjusted to reflect the correct server-side state.

- 💳 **Final Validation Before Payment**  
  Just before redirecting to the payment provider (e.g. Stripe), a **final stock check** is performed.  
  If a product's stock is insufficient to fulfill the quantity in the cart, the system:
  - Notifies the user
  - Automatically lowers the quantity to the highest possible amount

- 📉 **Stock Deduction After Payment Confirmation**  
  Stock is only reduced once payment is successfully confirmed.  
  This prevents stock loss from abandoned or failed payments and ensures that only finalized orders affect inventory.

This entire flow guarantees:
- Accurate pricing
- Real-time stock integrity
- A seamless shopping experience without confusing order failures

### 💡 User-Friendly Features

The platform includes a range of modern enhancements designed to make the shopping and checkout experience smooth, secure, and efficient for customers.

#### 💳 Stripe Payment Integration

The system uses **Stripe** to securely handle online payments.

- Users are redirected to a secure Stripe Checkout page
- All payment information is processed externally — no sensitive data is stored by the webshop
- Stripe supports multiple payment methods (e.g. cards, wallets, bank transfers depending on region)

#### 🔐 Social Login Options

To streamline account creation and login, users can authenticate using:

- ✅ **Facebook**
- ✅ **Google**

This saves time during checkout and improves onboarding for returning customers.

#### ⏳ Payment Grace Period

If a user creates an order but the payment **fails** or is **abandoned**, the system gives them a **1-hour window** to complete the payment.

After this time:
- The order is automatically deleted
- All reserved stock is restored
- This prevents inventory from being locked due to unpaid orders

#### 🛒 Cart Persistence After Logout

To enhance the user experience, the system **preserves the shopping cart** even after logout:

- Cart data is stored for up to **1 hour**
- When the user logs back in, their cart is restored automatically
- This prevents frustration from accidentally lost sessions and supports longer browsing sessions

