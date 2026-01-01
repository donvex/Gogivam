package main

import (
	"fmt"
	"github.com/donvex/Gogivam"
)

func main() {
	// Test the Greet function
	fmt.Println(gogivam.Greet("Gogivam"))
	fmt.Println(gogivam.Greet(""))
	fmt.Println(gogivam.Greet("Developer"))

	// Test the Add function
	fmt.Printf("\n2 + 3 = %d\n", gogivam.Add(2, 3))
	fmt.Printf("10 + (-4) = %d\n", gogivam.Add(10, -4))
	fmt.Printf("0 + 0 = %d\n", gogivam.Add(0, 0))
}
