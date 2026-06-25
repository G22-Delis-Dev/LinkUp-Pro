# 🛣️ Security Implementation Roadmap

## ✅ Completed
1. **Image Storage Service** - GUID naming, validation by magic numbers, 5MB limit
2. **Image Validator** - Content-based validation (JPEG, PNG, WebP)
3. **Architecture Review** - Onion Architecture compliance verified
4. **NET 9 Migration** - All projects upgraded

## 🚧 To Implement

### Phase 1: Core Security Infrastructure
- [ ] Create `ActiveAccountFilter` for account state validation
- [ ] Create `UserExtensions` with `GetUserId()` helper
- [ ] Configure Identity with password and lockout settings in `Program.cs`
- [ ] Configure session and cookie security in `Program.cs`
- [ ] Create error handling middleware
- [ ] Configure CSP headers

### Phase 2: ViewModels
- [ ] Auth ViewModels (Login, Register, ForgotPassword, ResetPassword, etc.)
- [ ] Post ViewModels (Create, Edit, Delete)
- [ ] Comment ViewModels
- [ ] FriendRequest ViewModels
- [ ] Profile ViewModels
- [ ] Notification ViewModels
- [ ] Battleship ViewModels

### Phase 3: Controllers with Security
- [ ] AuthController (public routes with anti-forgery)
- [ ] PostController (authorize + resource validation)
- [ ] CommentController (authorize + resource validation)
- [ ] FriendRequestController (authorize + resource validation)
- [ ] FriendshipController (authorize + resource validation)
- [ ] ProfileController (authorize + resource validation)
- [ ] NotificationController (authorize + resource validation)
- [ ] BattleshipController (authorize + game participation validation)

### Phase 4: Views with Security
- [ ] Add anti-forgery tokens to all forms
- [ ] Use `@Model.Property` for all user content (never Html.Raw)
- [ ] Add password visibility toggles
- [ ] Create error pages (404, 403, 500)

### Phase 5: Additional Security
- [ ] Email service for activation and password reset
- [ ] Custom token providers for different lifespans
- [ ] Concurrency control with RowVersion
- [ ] Authorization services for each module

## 📝 Implementation Order

**Start with:**
1. Program.cs configuration
2. ActiveAccountFilter
3. UserExtensions
4. ErrorHandlingMiddleware

**Then:**
5. Auth ViewModels and Controller
6. Test authentication flow

**Finally:**
7. Protected controllers one by one
8. Views with security features

## ⚠️ Important Notes

- ALL controllers except Auth should have `[Authorize]` at class level
- ALL POST actions must have `[ValidateAntiForgeryToken]`
- ALL resource operations must validate ownership/access
- NEVER expose sensitive data in logs or error messages
- Use ViewModels to prevent over-posting
- Server-side validation is MANDATORY
