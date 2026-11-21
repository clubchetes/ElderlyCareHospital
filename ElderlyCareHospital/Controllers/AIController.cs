using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ElderlyCareHospital.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Ask(string message)
        {
            try
            {
                // =====================================================
                // 1. CHECK USER MESSAGE
                // =====================================================

                if (string.IsNullOrWhiteSpace(message))
                {
                    return Json(new
                    {
                        success = false,
                        response = "Please enter a question."
                    });
                }


                // =====================================================
                // 2. GET GEMINI API KEY FROM WEB.CONFIG
                // =====================================================

                string apiKey =
                    System.Configuration.ConfigurationManager
                    .AppSettings["GeminiApiKey"];


                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return Json(new
                    {
                        success = false,
                        response =
                            "Gemini API key is missing from Web.config."
                    });
                }


                // =====================================================
                // 3. GEMINI MODEL
                // =====================================================

                string model = "gemini-3.6-flash";


                // =====================================================
                // 4. GEMINI API URL
                // =====================================================

                string url =
                    "https://generativelanguage.googleapis.com/" +
                    "v1beta/models/" +
                    model +
                    ":generateContent";


                // =====================================================
                // 5. ELDERLYCARE HOSPITAL AI PROMPT
                // =====================================================

                string prompt =
                    "You are the ElderlyCare Hospital AI Assistant. " +
                    "You are a warm, friendly, professional and helpful " +
                    "virtual assistant for the ElderlyCare Hospital " +
                    "management system. " +

                    "Your purpose is to help patients, family members, " +
                    "caregivers, healthcare staff and authorized users " +
                    "understand and navigate the ElderlyCare Hospital system. " +

                    "ABOUT ELDERLYCARE HOSPITAL: " +
                    "ElderlyCare Hospital is dedicated to providing " +
                    "compassionate, safe and professional care for elderly " +
                    "patients and residents. " +

                    "The hospital system helps healthcare staff, caregivers " +
                    "and authorized users securely access patient care services. " +

                    "The hospital's core values are caring with dignity, " +
                    "respect and compassion every day. " +

                    "HOSPITAL CONTACT INFORMATION: " +
                    "Phone: +27 12 345 6789. " +
                    "Emergency: +27 78 358 8239. " +
                    "Email: support@elderlycarehospital.co.za. " +
                    "Address: 25 Care Avenue, Pretoria, South Africa. " +

                    "WHAT YOU CAN HELP WITH: " +
                    "Explain how the ElderlyCare Hospital system works. " +
                    "Explain features such as patient management, " +
                    "appointments, visits, doctors, nurses, medication, " +
                    "messages, care plans, medical notes, emergency alerts " +
                    "and other hospital management features. " +

                    "The hospital has professional doctors. " +
                    "The nurses are strong, friendly, caring and supportive. " +
                    "The hospital provides high-quality medication and " +
                    "patient care. " +

                    "Help users understand general healthcare information " +
                    "using simple language. " +

                    "Help users find the appropriate hospital contact " +
                    "information when needed. " +

                    "PERSONALITY: " +
                    "Always be warm, welcoming, respectful, patient and friendly. " +

                    "Treat elderly patients and their family members with " +
                    "dignity and compassion. " +

                    "Use clear and simple language. " +
                    "Avoid unnecessarily technical language. " +
                    "Be professional but do not sound robotic or cold. " +
                    "Keep normal answers concise and helpful. " +
                    "You may use a small number of friendly emojis when appropriate. " +

                    "MEDICAL SAFETY: " +
                    "You are an AI assistant and not a doctor, nurse or " +
                    "medical professional. " +

                    "Never claim to diagnose a patient. " +
                    "Never replace professional medical advice. " +
                    "Do not recommend dangerous treatments or medication changes. " +

                    "For serious medical concerns, advise the user to speak " +
                    "with a qualified healthcare professional. " +

                    "For emergencies, advise the user to seek immediate " +
                    "emergency assistance and provide the hospital emergency " +
                    "number when appropriate. " +

                    "PRIVACY AND SECURITY: " +
                    "Protect patient privacy at all times. " +

                    "Never invent, guess or reveal private patient information. " +

                    "Never claim to access patient medical records, databases, " +
                    "appointments, prescriptions or other private information " +
                    "unless that information has explicitly been provided to you. " +

                    "ACCURACY: " +
                    "Never invent hospital policies, staff names, patient " +
                    "information, appointments, medical records or contact details. " +

                    "If you do not know an answer, say so honestly and direct " +
                    "the user to the appropriate hospital staff. " +

                    "GREETING: " +
                    "If the user says hello, hi, good morning, good afternoon " +
                    "or something similar, respond warmly and offer to help. " +

                    "Always represent ElderlyCare Hospital with compassion, " +
                    "dignity, respect, safety, professionalism and genuine " +
                    "willingness to help. " +

                    "\n\nUSER QUESTION: " +
                    message;


                // =====================================================
                // 6. CREATE GEMINI REQUEST BODY
                // =====================================================

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",

                            parts = new[]
                            {
                                new
                                {
                                    text = prompt
                                }
                            }
                        }
                    }
                };


                string json =
                    JsonConvert.SerializeObject(requestBody);


                // =====================================================
                // 7. SEND REQUEST TO GEMINI
                // =====================================================

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(
                        "x-goog-api-key",
                        apiKey
                    );


                    using (StringContent content =
                        new StringContent(
                            json,
                            Encoding.UTF8,
                            "application/json"))
                    {
                        HttpResponseMessage result =
                            await client.PostAsync(
                                url,
                                content
                            );


                        string responseJson =
                            await result.Content.ReadAsStringAsync();


                        // =================================================
                        // 8. GEMINI API ERROR
                        // =================================================

                        if (!result.IsSuccessStatusCode)
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini API Error (" +
                                    (int)result.StatusCode +
                                    "): " +
                                    responseJson
                            });
                        }


                        // =================================================
                        // 9. CHECK EMPTY RESPONSE
                        // =================================================

                        if (string.IsNullOrWhiteSpace(responseJson))
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini returned an empty response."
                            });
                        }


                        // =================================================
                        // 10. DESERIALIZE GEMINI RESPONSE
                        // =================================================

                        dynamic geminiResponse =
                            JsonConvert.DeserializeObject(
                                responseJson
                            );


                        if (geminiResponse == null)
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini returned an invalid response."
                            });
                        }


                        // =================================================
                        // 11. CHECK CANDIDATES
                        // =================================================

                        if (geminiResponse.candidates == null ||
                            geminiResponse.candidates.Count == 0)
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini did not return an answer."
                            });
                        }


                        // =================================================
                        // 12. CHECK CONTENT
                        // =================================================

                        if (geminiResponse.candidates[0].content == null)
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini returned a response without content."
                            });
                        }


                        if (geminiResponse
                            .candidates[0]
                            .content
                            .parts == null)
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini returned a response without text."
                            });
                        }


                        if (geminiResponse
                            .candidates[0]
                            .content
                            .parts
                            .Count == 0)
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini returned an empty text response."
                            });
                        }


                        // =================================================
                        // 13. GET AI TEXT
                        // =================================================

                        string aiResponse =
                            geminiResponse
                                .candidates[0]
                                .content
                                .parts[0]
                                .text;


                        if (string.IsNullOrWhiteSpace(aiResponse))
                        {
                            return Json(new
                            {
                                success = false,

                                response =
                                    "Gemini returned an empty answer."
                            });
                        }


                        // =================================================
                        // 14. RETURN SUCCESS
                        // =================================================

                        return Json(new
                        {
                            success = true,

                            response = aiResponse
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // =====================================================
                // 15. SHOW REAL ERROR WHILE TESTING
                // =====================================================

                return Json(new
                {
                    success = false,

                    response =
                        "AI Assistant Error: " +
                        ex.Message
                });
            }
        }
    }
}