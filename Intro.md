# ENKIVEDA Scaffolder Intro

Generally, ENKIVEDA Scaffolder is a developer tool, that helps us add new features to the Gear Guard backend in a standard, safe, and repeatable way.

So, Instead of manually creating and wiring every file, the Scaffolder follows the Gear Guard architecture and automates the common setup work.

Then we can focus more on the actual business logic.

---

## When it's comes to main features

* We can generate New function Slice Foundations.
* Enroll an existing handwritten slice into Scaffolder lifecycle.
* We can generate Capabilities, Functions.

---

## Scaffolder ownership split

Before start any operations you must need to know about Scaffolder ownership split

We can divide this into types, such as Developer owns and Scaffolder owns

### When its comes to Developer owns

- Domain entity
- Repository interface
- Repository implementation
- EF configuration
- Application commands and queries
- Application handlers
- Request models and mapping helpers
- Business errors and permissions
- Business rules and tests

### Scaffolder owns

- Slice foundation
- DI and DbContext wiring
- Architecture/test wiring
- HTTP Function adapter
- Function contract test
- Receipts, validation, doctor, rollback/upgrade lifecycle

---

Generally, The developer writes the business behavior. The Scaffolder doesn't invent business logic. It validates the reviewed code and generates only the standard technical wiring around it.”

---

Now we will move to operations.
