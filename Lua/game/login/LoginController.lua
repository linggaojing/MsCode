require("game.login.LoginModel")
require("game.login.LoginView")
LoginController = LoginController or BaseClass()

function LoginController:__init()
	self.LoginView = LoginView.New()
	self.LoginView:Open()
end