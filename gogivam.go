package gogivam

// Greet returns a greeting message
func Greet(name string) string {
	if name == "" {
		return "Hello, World!"
	}
	return "Hello, " + name + "!"
}

// Add adds two integers and returns the result
func Add(a, b int) int {
	return a + b
}
