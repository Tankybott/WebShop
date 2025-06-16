# 🛍️ Project Overview

## 📚 Table of Contents

- [🚀 Try It Yourself](#-try-it-yourself)
  
- [✨ Features & Functionalities](#-features--functionalities)
  - [📁 Category Management](#-category-management)
  - [🚚 Carrier Management](#-carrier-management)
  - [🛍️ Product & Discount Management](#-product--discount-management)
  - [⚙️ Webshop Settings](#-webshop-settings)
  - [👥 User Management](#-user-management)
  - [📦 Order Management](#-order-management)
    - [👤 User Order Access](#-user-order-access)
    - [🛠️ Admin Order Management](#-admin-order-management)
    - [📄 Order PDF Export](#-order-pdf-export)
  - [🔄 Stock & Price Lifecycle](#-stock--price-lifecycle)
  - [💡 User-Friendly Features](#-user-friendly-features)
    - [💳 Stripe Payment Integration](#-stripe-payment-integration)
    - [📧 Order Status Email Notifications](#-order-status-email-notifications)
    - [🔐 Social Login Options](#-social-login-options)
    - [⏳ Payment Grace Period](#-payment-grace-period)
    - [🛒 Cart Persistence After Logout](#-cart-persistence-after-logout)
      
- [🛠️ Technical Documentation](#-technical-documentation)
  - [⚙️ Tech Stack](#-tech-stack)
  - [🧱 Code Architecture](#-code-architecture)
  - [✅ Testing Strategy](#-testing-strategy)
  - [🧠 Some Code Optimizations](#-some-code-optimizations)
    - [💤 Low-Frequency Polling Services](#-1-low-frequency-polling-services)
    - [⚙️ High-Frequency Queue-Based Services](#-2-high-frequency-queue-based-services)
    - [🖼️ Optimized Image Processing](#-optimized-image-processing)
    - [🧭 Persistent Product Browsing State](#-persistent-product-browsing-state)

## 🚀 Try It Yourself

You can explore the app live here:  
🔗 **[http://webshop.hostingasp.pl/](http://webshop.hostingasp.pl/)**

### 🧪 Test Admin Account

Log in as a test administrator to preview the admin panel:  
- **Email:** `testadmin@mail.com`  
- **Password:** `Abcd123!`  

> ⚠️ This account has read-only access to admin features. You can preview everything, but you **cannot make changes**.

### 📝 Create Your Own Account

You're also welcome to register a new user account manually.  
📩 **Note:** Because there’s no custom domain email yet, confirmation emails may land in your **Spam folder** — please check there after signing up.

### 🛒 Want to Try a Purchase?

Feel free to place a test order using the Stripe demo card:  
💳 **Card Number:** `4242 4242 4242 4242`  
- Any valid expiration date
- Any 3-digit CVC

> 🔐 Google and Facebook login are currently only enabled for the owner (me) in this demo stage.

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

<p align="center">
  <img src="https://github.com/user-attachments/assets/ce1359bb-e0bf-402d-8448-d739403aea74" alt="Order Summary Admin Panel" width="80%" />
</p>

---

#### 📄 Order PDF Export

Each order has a **Download PDF** button available in the admin panel.

This allows for easy archiving or order sharing for administrative or logistical purposes.




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
- 
#### 📧 Order Status Email Notifications

To keep customers fully informed, the system automatically sends an **email notification** every time the status of their order changes.

Examples include:
- Order created
- Order moved to `Processing`
- Order has been `Shipped`

Emails include:
- Order summary
- Current status
- Tracking information (if available)

This ensures transparency and builds trust with the customer.

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

## 🛠️ Technical Documentation

### ⚙️ Tech Stack

This application was built using the following technologies:

- **Backend:** ASP.NET Core (MVC)
- **Database:** MySQL 
- **ORM:** Entity Framework Core
- **Frontend:** Razor Pages with TypeScript-enhanced behavior
- **Authentication:** ASP.NET Identity + external providers (Google, Facebook)
- **Payments:** Stripe Integration
- **Email:** SMTP with confirmation + order notifications
- **Hosting:** ASP.NET-compatible shared host (hostingasp.pl)

---

### 🧱 Code Architecture

This project was designed with a strong emphasis on:

- **Modularity & Clean Structure**  
  The system follows a **layer-based architecture**, clearly separating responsibilities across:
  - **Controllers** – handle request flow and input validation
  - **Services** – encapsulate business logic and workflows
  - **Repositories** – manage data access and queries
  - **Utilities** – handle cross-cutting concerns (e.g. PDF generation, image handling)
  - **Background Services** – manage tasks like order cleanup and stock restoration

- **Maintainability**  
  Code is structured to follow **SOLID principles** and **best practices in ASP.NET development**, making it easy to test, extend, and debug as the application grows.

- **Reusability**  
  Common logic such as:
  - Image uploading
  - Discount activation/expiration
  - Order stock adjustments
  - ViewModel generation  
  ...are encapsulated in reusable services and helpers to eliminate duplication.

- **Readability & Conventions**  
  Folder structure, naming conventions, and abstraction layers are consistently applied across the solution.  
  This helps any new developer onboard quickly and maintain a scalable codebase with ease.

---

### ✅ Testing Strategy

The **entire service layer** is fully covered with **unit tests**, designed to ensure long-term stability and confidence in business logic changes.

Key testing principles followed:

- **Proper mocking** of repositories, services, and external dependencies
- **Consistent naming convention**:  
  Each test method follows a `MethodName_ShouldExpectedBehavior_WhenCondition` format for clarity
- **One assertion per test**:  
  This ensures that each test checks only one behavior, making debugging fast and results easier to interpret

This disciplined testing approach supports clean development, simplifies onboarding, and ensures that even the most complex flows (like discount scheduling, order stock handling, or Stripe payment flows) remain reliable over time.

---

### 🧠 Some Code Optimizations
### ⚡ Some Code Optimizations

To ensure optimal performance, modularity, and scalability, the application utilizes **two distinct background service architectures**, each tailored to a specific type of workload:

---

#### 💤 1. Low-Frequency Polling Services  

📂 [`ProcessBackgroundService`](https://github.com/Tankybott/WebShop/tree/master/BackgroundServices/BackgroundProcessors)

This approach is ideal for **less time-sensitive tasks** that can be executed periodically without impacting user experience or data accuracy.

**Used for:**
- 🗑️ Removing expired carts
- 🧹 Deleting orders that have remained unconfirmed for too long

**How it works:**
- These services run at fixed intervals (e.g., every 30 or 60 minutes).
- Each service inherits from a shared base class `ProcessBackgroundService` which handles the scheduling.
- Subclasses only need to implement a `ProcessAsync()` method that encapsulates logic for fetching and processing stale data.

**Why it's optimal:**
- These operations are **infrequent** and can be handled with minimal performance cost.
- Database querying is done in **bulk**, reducing I/O frequency.
- Keeps the logic simple, clean, and **easy to extend** for similar use cases.

---

#### ⚙️ 2. High-Frequency Queue-Based Services  
📂 [`QueueProcessBackgroundService`](https://github.com/Tankybott/WebShop/tree/master/BackgroundServices/QueueProcessBackgroundServices)  
📂 [`Queue Implementations`](https://github.com/Tankybott/WebShop/tree/master/Utility/Queues)

This mechanism is designed for **time-critical tasks** that must react precisely to time-based triggers, such as scheduled discounts.

**Used for:**
- ✅ Activating discounts when their start time is reached
- ❌ Removing expired discounts when their end time is passed

**How it works:**
- Built on a generic `QueueProcessBackgroundService<TEntity>` abstract class
- Uses an in-memory `SortedQueue<T>` (based on `SortedSet<T>`) to sort items by time (start or end)
- Entities must implement `IHasId` to allow precise removal from the queue (e.g., if a discount is edited or deleted)
- O(1) access to the next item ensures we always process the **earliest eligible task first**

**Why it's optimal:**
- Real-time precision: No need to constantly poll the database
- **O(1) performance** for checking the next item due to the nature of `SortedSet`
- Queue remains consistently sorted after every enqueue
- **Trade-off:** Slight increase in memory usage in exchange for **massive performance gains and accuracy**

This structure is especially useful for dynamic, time-sensitive scenarios — like scheduled promotions — where real-time execution matters but constant DB hits would be wasteful.

#### 🖼️ Optimized Image Processing

To reduce load times and deliver responsive images across devices, the system uses server-side image processing at upload time.

Whenever a user uploads an image, the server:

1. **Generates a high-quality full-size image** (max 1600×1600)
2. **Creates a thumbnail version** (max 600×600) optimized for previews and lists

Both images are encoded in the **WebP format** using a custom encoder, offering:
- ✅ Modern compression
- ✅ Smaller file sizes
- ✅ High visual quality

This logic is handled by the `ImageProcessor` service, which:
- Uses the `SixLabors.ImageSharp` library for efficient image manipulation
- Works asynchronously to avoid blocking threads during file I/O
- Delegates file storage to a reusable `IFileService` for flexibility

#### 🧭 Persistent Product Browsing State

To improve user experience during product exploration, the app stores the user’s **last selected filters** in the browser’s **session storage**.

This means:
- When a user applies filters (e.g., category, sorting, search term) and navigates away from the product listing page...
- Then later **returns to the same page** (via navigation or browser back button)...
- The previously selected filters are **restored automatically**, so the user doesn’t need to reapply them

This feature provides:
- ✅ A smoother, more intuitive browsing experience
- ✅ Reduced frustration during long browsing sessions
- ✅ A modern feel that users expect from high-quality e-commerce platforms

