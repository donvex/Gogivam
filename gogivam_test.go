package gogivam

import "testing"

func TestGreet(t *testing.T) {
	tests := []struct {
		name     string
		input    string
		expected string
	}{
		{
			name:     "with name",
			input:    "Gogivam",
			expected: "Hello, Gogivam!",
		},
		{
			name:     "empty name",
			input:    "",
			expected: "Hello, World!",
		},
		{
			name:     "with another name",
			input:    "Developer",
			expected: "Hello, Developer!",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			result := Greet(tt.input)
			if result != tt.expected {
				t.Errorf("Greet(%q) = %q, want %q", tt.input, result, tt.expected)
			}
		})
	}
}

func TestAdd(t *testing.T) {
	tests := []struct {
		name     string
		a        int
		b        int
		expected int
	}{
		{
			name:     "positive numbers",
			a:        2,
			b:        3,
			expected: 5,
		},
		{
			name:     "negative numbers",
			a:        -5,
			b:        -3,
			expected: -8,
		},
		{
			name:     "mixed numbers",
			a:        10,
			b:        -4,
			expected: 6,
		},
		{
			name:     "zero values",
			a:        0,
			b:        0,
			expected: 0,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			result := Add(tt.a, tt.b)
			if result != tt.expected {
				t.Errorf("Add(%d, %d) = %d, want %d", tt.a, tt.b, result, tt.expected)
			}
		})
	}
}
