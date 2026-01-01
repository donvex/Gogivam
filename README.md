# Gogivam

A simple Go module demonstrating basic functionality with comprehensive tests.

## Features

- `Greet(name string)`: Returns a personalized greeting message
- `Add(a, b int)`: Adds two integers and returns the result

## Installation

```bash
go get github.com/donvex/Gogivam
```

## Usage

```go
package main

import (
    "fmt"
    "github.com/donvex/Gogivam"
)

func main() {
    // Greet function
    fmt.Println(gogivam.Greet("Gogivam"))  // Output: Hello, Gogivam!
    fmt.Println(gogivam.Greet(""))         // Output: Hello, World!
    
    // Add function
    result := gogivam.Add(5, 3)
    fmt.Println(result)                     // Output: 8
}
```

## Testing

Run the tests using:

```bash
go test -v
```

Run tests with coverage:

```bash
go test -v -cover
```

## Development

To contribute to this project:

1. Clone the repository
2. Make your changes
3. Run tests to ensure everything works
4. Submit a pull request