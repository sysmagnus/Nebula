package contactHandler

import (
	"encoding/json"
	"net/http"
	"sgc-server/database/contact"
	"strconv"
)

// LoadContacts endpoint cargar contactos.
func LoadContacts(w http.ResponseWriter, r *http.Request) {
	page := r.URL.Query().Get("page")
	search := r.URL.Query().Get("search")

	pagTemp, err := strconv.Atoi(page)
	if err != nil {
		http.Error(w, "Debe enviar el parámetro página como entero mayor a 0", http.StatusBadRequest)
		return
	}

	pag := int64(pagTemp)

	result, status := contact.FindContacts(pag, search)
	if status == false {
		http.Error(w, "Error al leer los contactos", http.StatusBadRequest)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	_ = json.NewEncoder(w).Encode(result)
}
